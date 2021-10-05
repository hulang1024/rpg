using Godot;

namespace Tools.CharacterStyleDesigner
{
    public class StyleComponentTabPanel : MarginContainer
    {
        private GridContainer gridContainer;

        public override void _Ready()
        {
            gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");
        }

        public void AddItem(StyleSelectItem item)
        {
            gridContainer.AddChild(item);
        }

        public void Clear()
        {
            foreach (var child in gridContainer.GetChildren())
                gridContainer.RemoveChild((Node)child);
        }

        public Godot.Collections.Array GetItems()
        {
            return gridContainer.GetChildren();
        }
    }
}
