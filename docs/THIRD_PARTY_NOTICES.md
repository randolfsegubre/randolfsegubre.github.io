# Third-party notices

This site makes no third-party requests when a visitor opens it. The only
third-party material is embedded in the source, listed here.

## Technology logos

- **What:** eleven technology logos (.NET, Umbraco, Vue.js, React, Blazor,
  Docker, Jenkins, MongoDB, MySQL, GitHub, Next.js), embedded as inline vector
  markup in `src/Portfolio.Web/Content/TechLogoData.cs`.
- **Source:** the Simple Icons package, version 16.32.0
  (<https://simpleicons.org>), released under the CC0 1.0 Universal
  public-domain dedication.
- **Trademarks:** the logos are trademarks of their respective owners. They are
  shown only to indicate which technologies the site owner has used, and do not
  imply endorsement.
- **The C# mark is not from that set.** Microsoft's C# logo is not part of the
  public-domain icon set, so the C# mark in `TechLogoData.cs` is a simple
  purple hexagon with the letters "C#", drawn for this site. It is a hand-drawn
  approximation, not the official artwork, and can be replaced with the
  official logo by editing that one entry.
- **How the file was made:** generated from the package by a small script that
  copies each icon's shape and brand color. To add a logo, add its technology
  name to `Profile.BackdropTags` (it must also appear in a project or role stack)
  and add an entry to `TechLogoData`.

## Portrait

The portrait `src/Portfolio.Web/wwwroot/images/randolf.webp` is the site
owner's own photograph, supplied by him.
