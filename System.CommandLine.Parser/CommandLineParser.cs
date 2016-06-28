
#region Using Directives

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.CommandLine.Parser.Antlr;
using System.CommandLine.Parser.ParameterConverters;
using System.CommandLine.Parser.Parameters;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace System.CommandLine.Parser
{
    /// <summary>
    /// Represents a parser, which is able to parse command line parameters and convert them to strongly-typed .NET data types.
    /// </summary>
    public class CommandLineParser
    {
        #region Private Static Fields

        /// <summary>
        /// Contains a map of all types supported for default parameters, that maps the type to a conversion method, which converts the original values into the specified collection type.
        /// </summary>
        private static IDictionary<Type, Func<IEnumerable<string>, object>> defaultParameterCollectionConversionMap = new Dictionary<Type, Func<IEnumerable<string>, object>>
        {
            [typeof(string)] = array => string.Join(" ", array),
            [typeof(StringBuilder)] = array => new StringBuilder(string.Join(" ", array)),

            [typeof(string[])] = array => array.ToArray(),
            [typeof(IList<string>)] = array => array.ToList(),
            [typeof(IEnumerable<string>)] = array => array.ToList(),
            [typeof(ICollection<string>)] = array => new Collection<string>(array.ToList()),
            [typeof(ISet<string>)] = array => new HashSet<string>(array),
            [typeof(IReadOnlyCollection<string>)] = array => new ReadOnlyCollection<string>(array.ToList()),
            [typeof(IReadOnlyList<string>)] = array => new ReadOnlyCollection<string>(array.ToList()),
            [typeof(List<string>)] = array => array.ToList(),
            [typeof(Collection<string>)] = array => new Collection<string>(array.ToList()),
            [typeof(LinkedList<string>)] = array => new LinkedList<string>(array),
            [typeof(Queue<string>)] = array => new Queue<string>(array),
            [typeof(SortedSet<string>)] = array => new SortedSet<string>(array),
            [typeof(HashSet<string>)] = array => new HashSet<string>(array),
            [typeof(Stack<string>)] = array => new Stack<string>(array),

            [typeof(StringBuilder[])] = array => array.Select(item => new StringBuilder(item)).ToArray(),
            [typeof(IList<StringBuilder>)] = array => array.Select(item => new StringBuilder(item)).ToList(),
            [typeof(IEnumerable<StringBuilder>)] = array => array.Select(item => new StringBuilder(item)).ToList(),
            [typeof(ICollection<StringBuilder>)] = array => new Collection<StringBuilder>(array.Select(item => new StringBuilder(item)).ToList()),
            [typeof(ISet<StringBuilder>)] = array => new HashSet<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(IReadOnlyCollection<StringBuilder>)] = array => new ReadOnlyCollection<StringBuilder>(array.Select(item => new StringBuilder(item)).ToList()),
            [typeof(IReadOnlyList<StringBuilder>)] = array => new ReadOnlyCollection<StringBuilder>(array.Select(item => new StringBuilder(item)).ToList()),
            [typeof(List<StringBuilder>)] = array => array.Select(item => new StringBuilder(item)).ToList(),
            [typeof(Collection<StringBuilder>)] = array => new Collection<StringBuilder>(array.Select(item => new StringBuilder(item)).ToList()),
            [typeof(LinkedList<StringBuilder>)] = array => new LinkedList<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(Queue<StringBuilder>)] = array => new Queue<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(SortedSet<StringBuilder>)] = array => new SortedSet<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(HashSet<StringBuilder>)] = array => new HashSet<StringBuilder>(array.Select(item => new StringBuilder(item))),
            [typeof(Stack<StringBuilder>)] = array => new Stack<StringBuilder>(array.Select(item => new StringBuilder(item)))
        };

        #endregion

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
            new CollectionOfNumberParameterConverter(),
            new CollectionOfBooleanParameterConverter(),
            new CollectionOfEnumerationParameterConverter(),
            new CollectionOfStringParameterConverter()
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

        #region Public Parameter Converter Registration Methods

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

        #endregion

        #region Public Parsing Methods

        /// <summary>
        /// Parses the specified command line parameters.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <returns>Returns the parsed parameters.</returns>
        public ParameterBag Parse(string commandLineParameters)
        {
            // Creates a new lexer and adds a custom error listener 
            LexerErrorListener lexerErrorListener = new LexerErrorListener();
            CommandLineLexer lexer = new CommandLineLexer(new AntlrInputStream(new StringReader(commandLineParameters)));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(lexerErrorListener);

            // Checks if any errors occurred during the lexing of the input
            if (lexerErrorListener.Errors.Any())
                throw new InvalidOperationException($"{lexerErrorListener.Errors.Count()} error(s) occurred during the lexing of the input.");

            // Creates a new parser and add a custom error listener
            ParserErrorListener parserErrorListener = new ParserErrorListener();
            Antlr.CommandLineParser parser = new Antlr.CommandLineParser(new CommonTokenStream(lexer)) { BuildParseTree = true };
            parser.RemoveErrorListeners();
            parser.AddErrorListener(parserErrorListener);

            // Checks if any errors occurred during the parsing of the input
            if (parserErrorListener.Errors.Any())
                throw new InvalidOperationException($"{parserErrorListener.Errors.Count()} error(s) occurred during the parsing of the input.");

            // Parses the command line parameters using the ANTRL4 generated parsers
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
        /// Parses the specified command line parameters asynchronously.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <returns>Returns the parsed parameters.</returns>
        public Task<ParameterBag> ParseAsync(string commandLineParameters) => Task.Run(() => this.Parse(commandLineParameters));
        
        #endregion

        #region Public Instantiation Methods

        /// <summary>
        /// Instantiates a new object. The constructor parameters are taken from the specified bag of parameters.
        /// </summary>
        /// <param name="parameterBag">The parameter bag from which the constructor parameters abre being taken.</param>
        /// <param name="returnType">The type of object that is to be instantiated.</param>
        /// <exception cref="InvalidOperationException">If no constructor whose parameter list can be satisfied could be found or an error occurred during the instantiation of the object, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns the created instance of the specified type.</returns>
        public object Instantiate(ParameterBag parameterBag, Type returnType)
        {
            // Determines the constructor, which is to be used for instantiating the specified type, the algorithm is greedy and uses the constructor with the most constructor arguments that can be matched from the parsed command line arguments
            ConstructorInfo chosenConstructorInfo = null;
            Dictionary<Attribute, Type> chosenConstructorParameterInfos = null;
            foreach (ConstructorInfo constructorInfo in returnType.GetConstructors()
                .Where(constructor => constructor.IsPublic && !constructor.IsStatic)
                .OrderByDescending(constructor => constructor.GetParameters().Count()))
            {
                // Creates a dictionary which can hold information about the parameters of the constructor and their types
                chosenConstructorParameterInfos = new Dictionary<Attribute, Type>();

                // Cycles over all the parameters of the constructor and gather information about them
                foreach (ParameterInfo constructorParameterInfo in constructorInfo.GetParameters())
                {
                    // Gets the default parameter attribute of the constructor parameter, if it has one, then it is matched with the default parameters
                    DefaultParameterAttribute defaultParameterAttribute = constructorParameterInfo.GetCustomAttribute<DefaultParameterAttribute>();
                    if (defaultParameterAttribute != null)
                    {
                        chosenConstructorParameterInfos.Add(defaultParameterAttribute, constructorParameterInfo.ParameterType);
                        continue;
                    }

                    // Gets the name of the command line parameter with which the constructor parameter is to be matched (which is either retrieved from the parameter name attribute or the name of the constructor parameter
                    ParameterNameAttribute parameterNameAttribute = constructorParameterInfo.GetCustomAttribute<ParameterNameAttribute>();
                    if (parameterNameAttribute != null)
                        chosenConstructorParameterInfos.Add(parameterNameAttribute, constructorParameterInfo.ParameterType);
                    else
                        chosenConstructorParameterInfos.Add(new ParameterNameAttribute(constructorParameterInfo.Name), constructorParameterInfo.ParameterType);
                }

                // Checks if the constructor can be used, which is when all constructor parameters can be matched with command line parameters
                bool canConstructorBeUsed = true;
                foreach (Attribute attribute in chosenConstructorParameterInfos.Keys)
                {
                    // Gets the type of the constructor parameter, which is to be used to match it to command line parameters
                    Type parameterType = chosenConstructorParameterInfos[attribute];

                    // Checks if the constructor parameter is to be matched with the default parameters
                    DefaultParameterAttribute defaultParameterAttribute = attribute as DefaultParameterAttribute;
                    if (defaultParameterAttribute != null)
                    {
                        // Only strings, string builders, or collection types are supported for default parameters
                        if (!CommandLineParser.defaultParameterCollectionConversionMap.ContainsKey(parameterType))
                        {
                            canConstructorBeUsed = false;
                            break;
                        }

                        // Since the constructor parameter has a valid type, the next constructor parameter can be tested
                        continue;
                    }

                    // Gets the command line parameter by the name, if no parameter could be found, then the constructor can not be used
                    ParameterNameAttribute parameterNameAttribute = attribute as ParameterNameAttribute;
                    if (!parameterBag.Parameters.ContainsKey(parameterNameAttribute.ParameterName) && (string.IsNullOrWhiteSpace(parameterNameAttribute.ParameterAlias) || !parameterBag.Parameters.ContainsKey(parameterNameAttribute.ParameterAlias)))
                    {
                        canConstructorBeUsed = false;
                        break;
                    }
                    Parameter parameter = parameterBag.Parameters[parameterNameAttribute.ParameterName];
                    if (parameter == null)
                        parameter = parameterBag.Parameters[parameterNameAttribute.ParameterAlias];

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
            foreach (Attribute attribute in chosenConstructorParameterInfos.Keys)
            {
                // Gets the type of the constructor parameter, which is to be used to match it to command line parameters
                Type constructorParameterType = chosenConstructorParameterInfos[attribute];

                // Checks if the constructor parameter is to be matched with a default parameter
                DefaultParameterAttribute defaultParameterAttribute = attribute as DefaultParameterAttribute;
                if (defaultParameterAttribute != null)
                {
                    // Converts the default parameters and sets them as the constructor parameter
                    IEnumerable<string> defaultParameters = parameterBag.DefaultParameters.OfType<DefaultParameter>().Select(defaultParameter => defaultParameter.Value);
                    constructorParameters[index++] = CommandLineParser.defaultParameterCollectionConversionMap[constructorParameterType](defaultParameters);

                    // Since the constructor parameter has already been found, the next constructor parameter can be processed
                    continue;
                }

                // Gets the matched command line parameter
                ParameterNameAttribute parameterNameAttribute = attribute as ParameterNameAttribute;
                Parameter parameter = null;
                parameterBag.Parameters.TryGetValue(parameterNameAttribute.ParameterName, out parameter);
                if (parameter == null)
                    parameter = parameterBag.Parameters[parameterNameAttribute.ParameterAlias];

                // Sets the constructor parameter
                constructorParameters[index++] = this.FindBestMatchingParameterConverter(constructorParameterType, parameter).Convert(constructorParameterType, parameter);
            }

            // Craetes a new instance of the specified type and validates whether it could be instantiated, if not then an exception is thrown
            object instance = chosenConstructorInfo.Invoke(constructorParameters);
            if (instance == null)
                throw new InvalidOperationException("No instance of the specified type could be constructed.");

            // Returns the created instance
            return instance;
        }

        /// <summary>
        /// Instantiates a new object asynchronously. The constructor parameters are taken from the specified bag of parameters.
        /// </summary>
        /// <param name="parameterBag">The parameter bag from which the constructor parameters abre being taken.</param>
        /// <param name="returnType">The type of object that is to be instantiated.</param>
        /// <exception cref="InvalidOperationException">If no constructor whose parameter list can be satisfied could be found or an error occurred during the instantiation of the object, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns the created instance of the specified type.</returns>
        public Task<object> InstantiateAsync(ParameterBag parameterBag, Type returnType) => Task.Run(() => this.Instantiate(parameterBag, returnType));

        /// <summary>
        /// Instantiates a new object. The constructor parameters are taken from the specified bag of parameters.
        /// </summary>
        /// <typeparam name="T">The type of object that is to be instantiated.</typeparam>
        /// <param name="parameterBag">The parameter bag from which the constructor parameters abre being taken.</param>
        /// <exception cref="InvalidOperationException">If no constructor whose parameter list can be satisfied could be found or an error occurred during the instantiation of the object, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns the created instance of the specified type.</returns>
        public T Instantiate<T>(ParameterBag parameterBag) where T : class => this.Instantiate(parameterBag, typeof(T)) as T;

        /// <summary>
        /// Instantiates a new object asynchronously. The constructor parameters are taken from the specified bag of parameters.
        /// </summary>
        /// <typeparam name="T">The type of object that is to be instantiated.</typeparam>
        /// <param name="parameterBag">The parameter bag from which the constructor parameters abre being taken.</param>
        /// <exception cref="InvalidOperationException">If no constructor whose parameter list can be satisfied could be found or an error occurred during the instantiation of the object, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns the created instance of the specified type.</returns>
        public Task<T> InstantiateAsync<T>(ParameterBag parameterBag) where T : class => Task.Run(() => this.Instantiate<T>(parameterBag));

        #endregion

        #region Public Injection Methods

        /// <summary>
        /// Injects the command line parameters from the parameter bag into the specified object instance.
        /// </summary>
        /// <param name="parameterBag">The parameter bag, which contains the command line parameters that are to be injected into an instance of the specified type.</param>
        /// <param name="instance">The object instance into which the command line parameters are to be injected.</param>
        public void Inject(ParameterBag parameterBag, object instance)
        {
            // Matches the public properties of the instance and injects all possible command line parameters into it
            foreach (PropertyInfo propertyInfo in instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.CanWrite))
            {
                // Gets the default parameter attribute of the property, which means that the property is to be matched with the default parameters
                DefaultParameterAttribute defaultParameterAttribute = propertyInfo.GetCustomAttribute<DefaultParameterAttribute>();
                if (defaultParameterAttribute != null)
                {
                    // Checks if the property has one of the types supported for defaut parameters, if not then the next property is processed
                    if (!CommandLineParser.defaultParameterCollectionConversionMap.ContainsKey(propertyInfo.PropertyType))
                        continue;

                    // Gets the default parameters and converts the to the type of the property
                    IEnumerable<string> defaultParameters = parameterBag.DefaultParameters.OfType<DefaultParameter>().Select(defaultParameter => defaultParameter.Value);
                    object convertedDefaultParameters = CommandLineParser.defaultParameterCollectionConversionMap[propertyInfo.PropertyType](defaultParameters);

                    // Injects the default parameters into the property
                    try
                    {
                        propertyInfo.SetMethod.Invoke(instance, new object[] { convertedDefaultParameters });
                    }
                    catch (InvalidOperationException) { }

                    // Since the propery has already been matched with the default parameters, the next property of the object can be processed
                    continue;
                }

                // Gets the name of the command line parameter with which the property is to be matched (which is either retrieved from the parameter name attribute or the name of the constructor parameter
                ParameterNameAttribute parameterNameAttribute = propertyInfo.GetCustomAttribute<ParameterNameAttribute>();
                if (parameterNameAttribute == null)
                    parameterNameAttribute = new ParameterNameAttribute(propertyInfo.Name);

                // Gets the command line parameter by the name, if no parameter could be found then the property can not be assigned
                if (!parameterBag.Parameters.ContainsKey(parameterNameAttribute.ParameterName) && (string.IsNullOrWhiteSpace(parameterNameAttribute.ParameterAlias) || !parameterBag.Parameters.ContainsKey(parameterNameAttribute.ParameterAlias)))
                    continue;
                Parameter parameter = null;
                parameterBag.Parameters.TryGetValue(parameterNameAttribute.ParameterName, out parameter);
                if (parameter == null)
                    parameter = parameterBag.Parameters[parameterNameAttribute.ParameterAlias];

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
        }

        /// <summary>
        /// Injects the command line parameters from the parameter bag into the specified object instance asynchronously.
        /// </summary>
        /// <param name="parameterBag">The parameter bag, which contains the command line parameters that are to be injected into an instance of the specified type.</param>
        /// <param name="instance">The object instance into which the command line parameters are to be injected.</param>
        public Task InjectAsync(ParameterBag parameterBag, object instance) => Task.Run(() => this.Inject(parameterBag, instance));

        #endregion

        #region Public Utility Methods

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <param name="returnType">The type that is to be instantiated and injected with the parameters from the command line.</param>
        /// <exception cref="InvalidOperationException">If no constructor whose parameter list can be satisfied could be found or an error occurred during the instantiation of the object, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public object Bind(string commandLineParameters, Type returnType)
        {
            // Parses the command line parameters
            ParameterBag parameterBag = this.Parse(commandLineParameters);

            // Instantiates a new object using the command line parameters and injects the command line parameters into it
            object instance = this.Instantiate(parameterBag, returnType);
            this.Inject(parameterBag, instance);

            // Returns the instantiated object
            return instance;
        }

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type asynchronously.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <param name="returnType">The type that is to be instantiated and injected with the parameters from the command line.</param>
        /// <exception cref="InvalidOperationException">If no constructor whose parameter list can be satisfied could be found or an error occurred during the instantiation of the object, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public async Task<object> BindAsync(string commandLineParameters, Type returnType)
        {
            // Parses the command line parameters
            ParameterBag parameterBag = await this.ParseAsync(commandLineParameters);

            // Instantiates a new object using the command line parameters and injects the command line parameters into it
            object instance = await this.InstantiateAsync(parameterBag, returnType);
            await this.InjectAsync(parameterBag, instance);

            // Returns the instantiated object
            return instance;
        }

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <typeparam name="T">The type that is to be instantiated and injected with the parameters from the command line.</typeparam>
        /// <exception cref="InvalidOperationException">If no constructor whose parameter list can be satisfied could be found or an error occurred during the instantiation of the object, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public T Bind<T>(string commandLineParameters) where T : class => this.Bind(commandLineParameters, typeof(T)) as T;

        /// <summary>
        /// Parses the specified command line parameters and converts them into the specified type asynchronously.
        /// </summary>
        /// <param name="commandLineParameters">The command line parameters that are to be parsed.</param>
        /// <typeparam name="T">The type that is to be instantiated and injected with the parameters from the command line.</typeparam>
        /// <exception cref="InvalidOperationException">If no constructor whose parameter list can be satisfied could be found or an error occurred during the instantiation of the object, then an <see cref="InvalidOperationException"/> exception is thrown.</exception>
        /// <returns>Returns an instance of the specified type injected with the parameters from the command line.</returns>
        public async Task<T> BindAsync<T>(string commandLineParameters) where T : class => await this.BindAsync(commandLineParameters, typeof(T)) as T;
        
        #endregion
    }
}