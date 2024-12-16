using System.Diagnostics.Contracts;

public static class Structs {
    public struct Highlight {
        public int highlightStart { get; set; }
        public int highlightEnd { get; set; }
        public int highlightMarker { get; set; }
        public byte highlightAlpha { get; set; }
        public byte highlightRed { get; set; }
        public byte highlightGreen { get; set; }
        public byte highlightBlue { get; set; }
    }

    public struct Skip {
        public int skip2 { get; set; }
        public int skip2Big { get; set; }
        public int skip3 { get; set; }
    }

    public struct ColourSub2 {
        public int colourSub2Start { get; set; }
        public int colourSub2End { get; set; }
        public int colourSub2Marker { get; set; }
        public byte colourSub2Alpha { get; set; }
        public byte colourSub2Red { get; set; }
        public byte colourSub2Green { get; set; }
        public byte colourSub2Blue { get; set; }
    }

    public struct ColourSub1 {
        public int colourSub1Start { get; set; }
        public int colourSub1End { get; set; }
        public int colourSub1Marker { get; set; }
        public byte colourSub1Alpha { get; set; }
        public byte colourSub1Red { get; set; }
        public byte colourSub1Green { get; set; }
        public byte colourSub1Blue { get; set; }
    }

    public struct ColourMain {
        public int colourMainStart { get; set; }
        public int colourMainEnd { get; set; }
        public int colourMainMarker { get; set; }
        public byte colourMainAlpha { get; set; }
        public byte colourMainRed { get; set; }
        public byte colourMainGreen { get; set; }
        public byte colourMainBlue { get; set; }
    }

    public struct Cell {
        public string CellName { get; set; }
        public byte[] CellNameWrite { get; set; }
        public int CellNameCharsCount { get; set; }
        public string CellMessage { get; set; }
        public byte[] CellMessageWrite { get; set; }
        public int MessageCharAmount { get; set; }
        public List<ColourMain> ColourMainList { get; set; }
        public List<ColourSub1> ColourSub1List { get; set; }
        public List<ColourSub2> ColourSub2List { get; set; }
        //public List<Skip> SkipList { get; set; }
        public int Alignment { get; set; }
        public List<Highlight> HighlightList { get; set; }
    }

    public struct Group {
        public string GroupName { get; set; }
        public int GroupNameCharsCount { get; set; }
        public List<Cell> CellList { get; set; }
    }

    public struct Texture {
        public string TextureName { get; set; }
        public int TextureSizeX { get; set; }
        public int TextureSizeY {get; set; }
    }

    public struct Character {
        public int TextureIndex { get; set; }
        public string CharID { get; set; }
        public float CharPoint1X { get; set; }
        public float CharPoint1Y { get; set; }
        public float CharPoint2X { get; set; }
        public float CharPoint2Y { get; set; }
    }
}