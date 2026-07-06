# CLI.NET Core — Brand & Logo Guide

This document describes the CLI.NET Core visual identity and how to use the logo assets in this repository. All assets live in this folder.

## Concept

The mark is an **aperture "C"** cradling a **dot**.

- The **C** stands for CLI, Core, and C#.
- The **dot** is the `.` of `.NET` — the pivot the whole ecosystem turns around.
- The opening of the C reads as an *intake*: arguments going in, structure coming out. It nods to the way ASP.NET Core routes a request to a handler — the same API feel CLI.NET Core mirrors for the command line.

The style is **restrained and terminal-native**: near-black and a single violet accent, geometric construction, no literal terminal window or blinking prompt.

## Color

| Token           | Hex                   | Use                                     |
| --------------- | --------------------- | --------------------------------------- |
| Ink             | `#23222B`             | Primary text, mark on light             |
| Ink (mark)      | `#4E4672`             | The "C" on light glass badges           |
| Accent (violet) | `#6D4AFF`             | The dot, the `.` in the wordmark, links |
| Accent (light)  | `#A98CFF` / `#8B6BFF` | Dot / accent on dark surfaces           |
| Paper           | `#F3F2F5`             | Mark on dark, light surfaces            |
| Near-black      | `#16151C`             | Dark surfaces, terminal chrome          |

**Dark badge field** (radial, light→dark): `#4A3D82 → #2C2650 → #1C1930` **Light badge field** (linear): `#EFEAFF → #E6DBFF → #F4ECFB`

The violet accent is the only saturated color. Use it sparingly — one accent per composition.

## Typography

**Space Grotesk** — wordmark and headings. The wordmark is weight **600** with `CLI` / `NET` in ink, the `.` in accent violet, and `Core` in weight **400**, slightly muted.

Wordmark spelling is always **`CLI.NET Core`** — the dot is colored, `Core` is a separate lighter word.

## The Mark

- Outer circle radius **39**, inner radius **21**, centered in a 100×100 box.
- Mouth opening on the right, spanning **±34°**.
- The four mouth terminals are filleted with an **equal corner radius of 3** (mathematically tangent to the arcs — no visible kink).
- The dot sits at **(73, 50)** with radius **8**, in the accent violet.

Do not re-draw the mark by hand; use the vector files below.

## Variants & Files

### Core Vector Marks

- `mark-light.svg` — ink mark + violet dot, for light backgrounds
- `mark-dark.svg` — paper mark + light-violet dot, for dark backgrounds

### App/Package Badges

- `badge-glass-light.svg` / `-dark.svg` — the **primary** frosted-glass badge (drop shadow on the inner panel, inner glow, gentle gradient dither).
- `badge-flat-light.svg` / `-dark.svg` — flat, full-bleed badges. Use for small favicons and anywhere filter effects may be stripped.

### Wordmark Lockups

- `wordmark-light.svg` / `-dark.svg` — compact bar (mark + text).
- `readme-light.svg` / `-dark.svg` — tall horizontal lockup for READMEs.
- `wordmark-bars.html` — responsive, fill-width HTML version of the bar (`.clinet-core-bar`, add `.clinet-core-bar--dark` on dark surfaces).

## Using the Logo on GitHub

GitHub sanitizes SVG `<filter>` effects when rendered via `<img>`, so use the **baked PNGs** for the glass look, and let GitHub pick light/dark:

```markdown
![CLI.NET Core Logo](design/readme-header-dark.png#gh-dark-mode-only) ![CLI.NET Core Logo](design/readme-header-light.png#gh-light-mode-only)
```

## Dos/Don'ts

**Dos:**

- Keep clear space around the mark equal to the dot's diameter.
- Pair the light mark with light surfaces, the dark mark with dark surfaces.
- Use the flat badge when effects might not render (tiny sizes, sanitized SVG).

**Don'ts:**

- Recolor the mark outside the palette, or add a second accent.
- Stretch or skew the mark or wordmark.
- Add a literal terminal window, cursor, or prompt.
- Rasterize the glass SVG very large — an 8-bit gradient will band; prefer the vector master or a super-sampled PNG.

## Notes on Formats

- **SVG** is the source of truth (resolution-independent). The glass badges use only widely-supported filter primitives (`feGaussianBlur`, `feOffset`, `feComposite`, `feFlood`, `feTurbulence`) — they open correctly in Inkscape.
- The SVG wordmarks reference the **Space Grotesk** font; install it or request an outlined version if you need them fully self-contained.
