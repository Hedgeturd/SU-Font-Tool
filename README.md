# Sonic Unleashed Font Tool
A tool to convert Unleashed Font-Texture (FTE) and Font-Converse (FCO) files into XML files and vice versa.

## Usage
```SonicUnleashedFCOConv <Path to .fte/.fco/.xml>```<br>
You can also simply drag and drop supported file formats onto the executable.

## Formats
### Font-Texture
FTE file formats contain data which help assign texture sprites to character slots to be used by the FCO in game.<br>
When converted to an XML you can view and edit the different fields to your liking.<br>
(I'm yet to give a proper explanation to these fields)

### Font-Converse
FCO file formats contain data such as text and cell properties to be used by the game when text needs to be called and displayed.<br>
When converted to an XML, you can view and edit the different fields such as:
- Table which define the translation table being used,
- Groups which holds Cells,
- Cell Messages that contain text data,
- Cell ColourMain that control the text colour,
- Cell Highlights which allows for more than one text colour.

### XML
Once happy with your changes, you can drag and drop your XML file onto the Program to turn it back into an FTE/FCO file

## To Do
- Update translation jsons for new system
- (Try) Add DDS Support

## Thanks
Big thank you to [TheExentist151](https://github.com/TheExentist151) for letting me recycle some of the code from [SonicColorsXTBConv](https://github.com/TheExentist151/SonicColorsXTBConv)<br>
Plus a thank you to [NextinHKRY](https://github.com/NextinMono) for general coding help

## Requirements
If the tool does nothing when used, you may need to install the [.NET Runtime](https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&rid=win10-x64&apphost_version=8.0) package that the tool needs to work
