# REEEEEEGen

This is dumb.

- Cell sizes and padding are hardcoded
- Charset is C;C from SciAdv.Net, same build we use for sc3tools in cc-patch
- **Compound characters are not supported**
- Font textures and widths are embedded
- To use, put all lines in an `input.txt` in working directory, `line%d.png` files will be generated.
- Pipe character (`|`) designates tips - in `|Foo| bar`, Foo will be tip colored.