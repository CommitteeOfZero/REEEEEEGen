# REEEEEEGen

This is dumb.

- Cell sizes and padding are hardcoded
- Charset is C;C from SciAdv.Net, same build we use for sc3tools in cc-patch
- **Compound characters are not supported**
- Font textures and widths are embedded
- **Current font textures are not the same as the ones I have in my dev install** (not sure which are the right ones)
- We're replacing them after mipmapping anyway
- To use, put all lines in an `input.txt` in working directory, `line%d.png` files will be generated.
- Pipe character (`|`) designates tips - in `|Foo| bar`, Foo will be tip colored.