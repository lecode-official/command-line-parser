
#region Using Directives

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using System.CommandLine.Parser.Antlr;
using System.CommandLine.Parser.ParameterConverters;
using System.CommandLine.Parser.Parameters;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#endregion

namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a parser, which is able to parse command line parameters and convert them to strongly-typed .NET data types.
    /// </summary>
    public class Parser
    {
        #region Private Fields

        /// <summary>
        /// Contains the parameter converters, which are used to convert command line parameters to actual values.
        /// </summary>
        private List<IParameterConverter> parameterConverters = new List<IParameterConverter>
        {
            new NumberParameterConverter(),
            new BooleanParameterConverter(),
            new EnumerationParameterConverter(),
            new StringParameterConverter(),
            new ArrayOfStringParameterConverter(),
            new ArrayOfBooleanParameterConverter()
        };

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds the paramter converter, which is able to convert the specified parameter to the specified type.
        /// </summary>
        /// <param name="propertyType">The type of the property to which the parameter value is to be converted.</param>
        /// <param name="parameter"></param>
        /// <returns>Returns the parameter converter, which is able to convert the specified parameter into the specified type. If no matching parameter converter could be found, then <c>null</c> is returned.</returns>
        private IParameterConverter FindBestMatchingParameterConverter(Type propertyType, Parameter parameter) => this.parameterConverters.FirstOrDefault(parameterConverter => parameterConverter.CanConvert(propertyType, parameter));
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a new parameter converter with the parser.
        /// </summary>
        /// <param name="parameterConverter">The parameter converter, which is to be registered.</param>
        public void RegisterParameterConverter(IParameterConverter parameterConverter) => this.parameterConverters.Insert(0, parameterConverter);

        /// <summary>
        /// Unregisters the specified parameter converter.
        /// </summary>
        /// <param name="parameterConverter">The parameter converter that is to be unregistered.</param>
        public void UnregisterParameterConverter(IParameterConverter parameterConverter) => this.parameterConverters.Remove(parameterConverter);

        /// <summary>
        /// Parses the specified command line parameters.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <returns>Returns the parsed parameters.</returns>
        public ParameterBag Parse(string commandLineParameters)
        {
            // Parses the command line parameters using the ANTRL4 generated parsers
            CommandLineLexer lexer = new CommandLineLexer(new AntlrInputStream(new StringReader(commandLineParameters)));
            CommandLineParser parser = new CommandLineParser(new CommonTokenStream(lexer)) { BuildParseTree = true };
            IParseTree parseTree = parser.commandLine();
            CommandLineVisitor commandLineVisitor = new CommandLineVisitor();
            commandLineVisitor.Visit(parseTree);

            // Returns the parsed parameters wrapped in a parameter bag
            return new ParameterBag
            {
                CommandLineParameters = commandLineParameters,
                Parameters = commandLineVisitor.Parameters,
                DefaultParameters = commandLineVisitor.DefaultParameters
            };
        }

        /// <summary>
        /// Parses the command line parameters that have been passed to the program.
        /// </summary>
        /// <returns>Returns the parsed parameters.</returns>
        public ParameterBag Parse() => this.Parse(Environment.CommandLine);

        /// <summary>
        /// Parses the specified command line parameters asynchronously.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <returns>Returns the parsed parameters.</returns>
        public Task<ParameterBag> ParseAsync(string commandLineParameters) => Task.Run(() => this.Parse(commandLineParameters));

        /// <summary>
        /// Parses the command line parameters that have been passed to the program asynchronously.
        /// </summary>
        /// <returns>Returns the parsed parameters.</returns>
        public Task<ParameterBag> ParseAsync() => Task.Run(() => this.Parse());

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <param name="returnType">The type that is to be instantiated and injected with the parameters from the command line.</param>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public object Parse(string commandLineParameters, Type returnType)
        {
            // Parses the command line parameters
            ParameterBag parameterBag = this.Parse(commandLineParameters);
            
            // Determines the constructor, which is to be used for instantiating the specified type, the algorithm is greedy and uses the constructor with the most constructor arguments that can be matched from the parsed command line arguments
            ConstructorInfo chosenConstructorInfo = null;
            Dictionary<string, Type> chosenConstructorParameterInfos = null;
            foreach (ConstructorInfo constructorInfo in returnType.GetConstructors()
                .Where(constructor => constructor.IsPublic && !constructor.IsStatic)
                .OrderByDescending(constructor => constructor.GetParameters().Count()))
            {
                // Creates a dictionary which can hold information about the parameters of the constructor and their types
                chosenConstructorParameterInfos = new Dictionary<string, Type>();

                // Cycles over all the parameters of the constructor and gather information about them
                foreach (ParameterInfo constructorParameterInfo in constructorInfo.GetParameters())
                {
                    // Gets the name of the command line parameter with which the constructor parameter is to be matched (which is either retrieved from the parameter name attribute or the name of the constructor parameter
                    string parameterName;
                    ParameterNameAttribute parameterNameAttribute = constructorParameterInfo.GetCustomAttribute<ParameterNameAttribute>();
                    if (parameterNameAttribute != null)
                        parameterName = parameterNameAttribute.ParameterName;
                    else
                        parameterName = constructorParameterInfo.Name;

                    // Adds the constructor parameter information to the parameter infos
                    chosenConstructorParameterInfos.Add(parameterName, constructorParameterInfo.ParameterType);
                }

                // Checks if the constructor can be used, which is when all constructor parameters can be matched with command line parameters
                bool canConstructorBeUsed = true;
                foreach (string parameterName in chosenConstructorParameterInfos.Keys)
                {
                    // Gets the type of the constructor parameter, which is to be used to match it to command line parameters
                    Type parameterType = chosenConstructorParameterInfos[parameterName];

                    // Gets the command line parameter by the name, if no parameter could be found, then the constructor can not be used
                    if (!parameterBag.Parameters.ContainsKey(parameterName))
                    {
                        canConstructorBeUsed = false;
                        break;
                    }
                    Parameter parameter = parameterBag.Parameters[parameterName];

                    // Checks if there is a parameter converter that is able to convert the parameter into the type of the constructor parameter
                    if (this.FindBestMatchingParameterConverter(parameterType, parameter) == null)
                    {
                        canConstructorBeUsed = false;
                        break;
                    }
                }

                // Checks if the constructor can be used, if so then the other constructors do not need to be checked
                if (canConstructorBeUsed)
                {
                    chosenConstructorInfo = constructorInfo;
                    break;
                }
            }

            // Checks if a constructor could be found, if not then an exception is thrown
            if (chosenConstructorInfo == null)
                throw new InvalidOperationException("No valid constructor could be found for the specified type.");

            // Prepares the constructor arguments
            object[] constructorParameters = new object[chosenConstructorParameterInfos.Count];
            int index = 0;
            foreach (string constructorParameterName in chosenConstructorParameterInfos.Keys)
            {
                // Gets the matched command line parameter
                Type constructorParameterType = chosenConstructorParameterInfos[constructorParameterName];
                Parameter parameter = parameterBag.Parameters[constructorParameterName];

                // Sets the constructor parameter
                constructorParameters[index++] = this.FindBestMatchingParameterConverter(constructorParameterType, parameter).Convert(constructorParameterType, parameter);
            }

            // Craetes a new instance of the specified type and validates whether it could be instantiated, if not then an exception is thrown
            object instance = chosenConstructorInfo.Invoke(constructorParameters);
            if (instance == null)
                throw new InvalidOperationException("No instance of the specified type could be constructed.");

            // Matches the public properties of the instance and injects all possible command line parameters into it
            foreach (PropertyInfo propertyInfo in returnType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.CanWrite))
            {
                // Gets the name of the command line parameter with which the property is to be matched (which is either retrieved from the parameter name attribute or the name of the constructor parameter
                string propertyName;
                ParameterNameAttribute parameterNameAttribute = propertyInfo.GetCustomAttribute<ParameterNameAttribute>();
                if (parameterNameAttribute != null)
                    propertyName = parameterNameAttribute.ParameterName;
                else
                    propertyName = propertyInfo.Name;

                // Gets the command line parameter by the name, if no parameter could be found then the property can not be assigned
                if (!parameterBag.Parameters.ContainsKey(propertyName))
                    continue;
                Parameter parameter = parameterBag.Parameters[propertyName];

                // Checks if there is a parameter converter, that is able to convert the parameter value to the type of the property
                IParameterConverter parameterConverter = this.FindBestMatchingParameterConverter(propertyInfo.PropertyType, parameter);
                if (parameterConverter == null)
                    continue;

                // Assigns the command line parameter value to the property
                try
                {
                    propertyInfo.SetMethod.Invoke(instance, new object[] { parameterConverter.Convert(propertyInfo.PropertyType, parameter) });
                }
                catch (InvalidOperationException) { }
            }

            // Returns the created instance of the specified type
            return instance;
        }

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type.
        /// </summary>
        /// <param name="returnType">The type that is to be instantiated and injected with the parameters from the command line.</param>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public object Parse(Type returnType) => this.Parse(Environment.CommandLine, returnType);

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <typeparam name="T">The type that is to be instantiated and injected with the parameters from the command line.</typeparam>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public T Parse<T>(string commandLineParameters) where T : class => this.Parse(commandLineParameters, typeof(T)) as T;

        /// <summary>
        /// Parses the command line parameters that have been passed to the program and converts them into the specified type.
        /// </summary>
        /// <typeparam name="T">The type that is to be instantiated and injected with the parameters from the command line.</typeparam>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public T Parse<T>() where T : class => this.Parse<T>(Environment.CommandLine);

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type asynchronously.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <param name="returnType">The type that is to be instantiated and injected with the parameters from the command line.</param>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public Task<object> ParseAsync(string commandLineParameters, Type returnType) => Task.Run(() => this.Parse(commandLineParameters, returnType));

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type asynchronously.
        /// </summary>
        /// <param name="returnType">The type that is to be instantiated and injected with the parameters from the command line.</param>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public Task<object> ParseAsync(Type returnType) => Task.Run(() => this.Parse(Environment.CommandLine, returnType));

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type asynchronously.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <typeparam name="T">The type that is to be instantiated and injected with the parameters from the command line.</typeparam>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public Task<T> ParseAsync<T>(string commandLineParameters) where T : class => Task.Run(() => this.Parse<T>(commandLineParameters));

        /// <summary>
        /// Parses the command line parameters that have been passed to the program and converts them into the specified type asynchronously.
        /// </summary>
        /// <typeparam name="T">The type that is to be instantiated and injected with the parameters from the command line.</typeparam>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public Task<T> ParseAsync<T>() where T : class => Task.Run(() => this.Parse<T>());

        #endregion
    }
}