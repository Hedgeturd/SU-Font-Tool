# Sonic Unleashed Font Tool
A tool to convert Unleashed Font-Texture (FTE) and Font-Converse (FCO) files into XML files and vice versa.<br>
There is a new tool with a GUI called [Converse](https://github.com/NextinMono/converse) which can also be used along side SU Font Tool.

## Usage
`SU Font Tool vX.X\nUsage: SUFontTool <Path to .fte/.fco/.xml>`\
You can also simply drag and drop supported file formats onto the executable.

## To Do
- (Try) Add DDS Support

## Thanks
Big thank you to [TheExentist151](https://github.com/TheExentist151) for letting me recycle some of the code from [SonicColorsXTBConv](https://github.com/TheExentist151/SonicColorsXTBConv)<br>
Plus a HUGE thank you to [NextinHKRY](https://github.com/NextinMono) for all of your coding help!!

## Requirements
If the tool does nothing when used, you may need to install the [.NET Runtime](https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&rid=win10-x64&apphost_version=8.0) package that the tool needs to work

***

## Formats
### Font-Texture (Experimental)
FTE file formats contain data which help assign texture sprites to character slots to be used by the FCO in game.\
When converted to an XML you can view and edit the different fields to your liking.
```xml
<FTE>
  <Textures>        - A list of Usable Textures in the Scene Language archive 
    <Texture Name="mat_comon_x360_002" Size_X="512" Size_Y="512" />
    <Texture Name="mat_comon_x360_002" Size_X="512" Size_Y="512" />
    <Texture Name="fte_ConverseMain_000" Size_X="512" Size_Y="512" />
  </Textures>
  <Characters>      - Mappings of each Character within their Texture file
    <Character TextureIndex="0" CharacterID="100" TopLeft_X="0" TopLeft_Y="0" BottomRight_X="28" BottomRight_Y="28" />
    <Character TextureIndex="0" CharacterID="101" TopLeft_X="30" TopLeft_Y="0" BottomRight_X="58" BottomRight_Y="28" />
    <Character TextureIndex="0" CharacterID="102" TopLeft_X="60" TopLeft_Y="0" BottomRight_X="88" BottomRight_Y="28" />
    <Character TextureIndex="0" CharacterID="103" TopLeft_X="90" TopLeft_Y="0" BottomRight_X="118" BottomRight_Y="28" />
    ...         - There are a lot more + You can assign your own too as long as it's
                                         outside the used characters and within the overall character limit
  </Characters>
</FTE>
```

### Font-Converse
FCO file formats contain data such as text and cell properties to be used by the game when text needs to be called and displayed.\
When converted to an XML, you can view and edit the different fields such as:
```xml
<FCO Table="Languages/English/Retail/WorldMap">     - Table which define the Translation Table to be used
  <Groups>
    <Group Name="wm_hint_start">                    - Groups which hold Cells
      <Cell Name="smile" Alignment="0">             - Cells that contain Text Message and Format data
        <Message>I made you this World Map. I\nthought it might come in handy.</Message>
        <ColourMain Start="0" End="59" Marker="2" Alpha="1" Red="1" Green="1" Blue="1" />
        <ColourSub1 Start="0" End="59" Marker="1" Alpha="0" Red="0" Green="0" Blue="0.10980392" />
        <ColourSub2 Start="0" End="59" Marker="0" Alpha="0" Red="0" Green="0" Blue="0.003921569" />
        <Highlights>
          <Highlight0 Start="0" End="14" Marker="2" Alpha="1" Red="1" Green="1" Blue="1" />
          <Highlight1 Start="15" End="24" Marker="2" Alpha="1" Red="0.9882353" Green="0.9529412" Blue="0.019607844" />
          <Highlight2 Start="25" End="25" Marker="2" Alpha="1" Red="0.8" Green="1" Blue="0.8" />
          <Highlight3 Start="26" End="59" Marker="2" Alpha="1" Red="1" Green="1" Blue="1" />
        </Highlights>
        <SubCells />
      </Cell>
      ...       - You can have any amount of Cells within a Group
    </Group>
    ...         - As well as Groups within a file
  </Groups>
</FCO>
```
