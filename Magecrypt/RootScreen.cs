using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RogueSharp;
using SadConsole.Input;
using Point = SadRogue.Primitives.Point;

namespace Magecrypt;

class RootScreen : ScreenObject
{
    private Map _map;
    private ScreenSurface _statusBar;
    private ScreenSurface _inventoryScreen;
    private ScreenSurface _targetingScreen;
    private ScreenSurface _targetingCursorScreen;
    private ScreenSurface _fieldOfViewScreen;
    private Point _targetingCursorPosition;
    private bool _targetingActive = false;

    private List<string> _messages = new List<string>();

    private bool _showingUseInventory = false;

    public RootScreen()
    {
        Instance = this;
        if (Instance != this) throw new Exception("Only one instance of RootScreen is allowed.");

        _targetingScreen = new ScreenSurface(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY);
        Children.Add(_targetingScreen);

        _map = new Map(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY - 5);
        Children.Add(_map.SurfaceObject);

        _fieldOfViewScreen = new ScreenSurface(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY);
        Children.Add(_fieldOfViewScreen);

        _statusBar = new ScreenSurface(Game.Instance.ScreenCellsX, 10);
        _statusBar.Position = new Point(0, Game.Instance.ScreenCellsY - 10);
        Children.Add(_statusBar);

        _inventoryScreen = new ScreenSurface(Game.Instance.ScreenCellsX - 10, Game.Instance.ScreenCellsY - 20);
        _inventoryScreen.Position = new Point(5, 5);
        Children.Add(_inventoryScreen);

        _targetingCursorScreen = new ScreenSurface(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY);
        Children.Add(_targetingCursorScreen);
    }

    public static RootScreen Instance { get; set; }

    public void Tick()
    {
        _map.Tick();
        UpdateFieldOfView();
    }

    public override void Render(TimeSpan delta)
    {
        _map.Draw();

        PrintStatusBar();

        base.Render(delta);
    }

    private void PrintStatusBar()
    {
        _statusBar.Clear();
        _statusBar.Print(1, 7, $"Gold: {Map.Instance.Player.Gold}", Color.White, Color.Black);

        string inventory = "[I]nventory: ";
        foreach (Item item in Map.Instance.Player.Inventory)
        {
            inventory += item.Id + ", ";
        }
        if (Map.Instance.Player.Inventory.Count > 0)
            inventory = inventory.Substring(0, inventory.Length - 2);
        _statusBar.Print(1, 9, inventory, Color.White, Color.Black);
        // Print equipped items
        _statusBar.Print(1, 8, $"Weapon: {Map.Instance.Player.Weapon?.Id ?? "None"}", Color.White, Color.Black);
        string armorString = "Armor: ";
        if (Map.Instance.Player.Armor != null && Map.Instance.Player.Armor.Count > 0)
        {
            foreach (KeyValuePair<Armor.ArmorSlot, Armor> armor in Map.Instance.Player.Armor)
            {
                armorString += $"{armor.Value.Id}: {armor.Value.Defense}, ";
            }

            armorString = armorString.Substring(0, armorString.Length - 2);
        }
        else
        {
            armorString += "None";
        }

        _statusBar.Print(40, 8, armorString, Color.White, Color.Black);
        _statusBar.Print(40, 7, $"Health: {Map.Instance.Player.Health}", Color.White, Color.Black);
        _statusBar.Print(60, 7, $"Mana: {Map.Instance.Player.Mana}", Color.White, Color.Black);
        _statusBar.Print(80, 7, $"Attack: {Map.Instance.Player.AttackValue}", Color.White, Color.Black);
        _statusBar.Print(100, 7, $"Defense: {Map.Instance.Player.DefenseValue}", Color.White, Color.Black);

        for (int i = 0; i < _messages.Count; i++)
        {
            Color messageColor = i == 0 ? Color.Red : Color.Yellow;
            messageColor = messageColor.SetAlpha((byte)(255 - (i * 40)));
            int index = _messages.Count - 1 - i;
            if (index >= 0 && index < _messages.Count)
                _statusBar.Print(1, 5 - i, _messages[index], messageColor, Color.Black);
        }

        UpdateFieldOfView();
    }

    public override void Update(TimeSpan delta)
    {
    }

    private bool ActivateItem(int index)
    {
        if (Map.Instance.Player.Inventory.Count >= index + 1)
        {
            if (Map.Instance.Player.Inventory[index].Usable)
                Map.Instance.Player.Inventory[index].Use(Map.Instance.Player);
            else if (Map.Instance.Player.Inventory[index].Equippable)
                Map.Instance.Player.Inventory[index].Equip(Map.Instance.Player);

            _showingUseInventory = false;
            _inventoryScreen.Clear();
            return true;
        }

        return false;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        bool handled = false;

        if (_targetingActive)
        {
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                HideTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.Up))
            {
                _targetingCursorPosition += Direction.Up;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                _targetingCursorPosition += Direction.Down;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.Left))
            {
                _targetingCursorPosition += Direction.Left;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.Right))
            {
                _targetingCursorPosition += Direction.Right;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad8))
            {
                _targetingCursorPosition += Direction.Up;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad2))
            {
                _targetingCursorPosition += Direction.Down;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad4))
            {
                _targetingCursorPosition += Direction.Left;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad6))
            {
                _targetingCursorPosition += Direction.Right;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad7))
            {
                _targetingCursorPosition += Direction.UpLeft;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad9))
            {
                _targetingCursorPosition += Direction.UpRight;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad1))
            {
                _targetingCursorPosition += Direction.DownLeft;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad3))
            {
                _targetingCursorPosition += Direction.DownRight;
                UpdateLineOfSightTargeting();
            }
            else if (keyboard.IsKeyPressed(Keys.Enter))
            {
                HideTargeting();
                handled = true;
            }
        }
        else if (_showingUseInventory)
        {
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                _showingUseInventory = false;
                _inventoryScreen.Clear();
            }
            else if (keyboard.IsKeyPressed(Keys.D1))
            {
                if (ActivateItem(0))
                {
                    handled = true;
                }
            }
            else if (keyboard.IsKeyPressed(Keys.D2))
            {
                if (ActivateItem(1))
                {
                    handled = true;
                }
            }
            else if (keyboard.IsKeyPressed(Keys.D3))
            {
                if (ActivateItem(2))
                {
                    handled = true;
                }
            }
            else if (keyboard.IsKeyPressed(Keys.D4))
            {
                if (ActivateItem(3))
                {
                    handled = true;
                }
            }
            else if (keyboard.IsKeyPressed(Keys.D5))
            {
                if (ActivateItem(4))
                {
                    handled = true;
                }
            }
            else if (keyboard.IsKeyPressed(Keys.D6))
            {
                if (ActivateItem(5))
                {
                    handled = true;
                }
            }
            else if (keyboard.IsKeyPressed(Keys.D7))
            {
                if (ActivateItem(6))
                {
                    handled = true;
                }
            }
            else if (keyboard.IsKeyPressed(Keys.D8))
            {
                if (ActivateItem(7))
                {
                    handled = true;
                }
            }
            else if (keyboard.IsKeyPressed(Keys.D9))
            {
                if (ActivateItem(8))
                {
                    handled = true;
                }
            }
            else
            {
                handled = false;
            }
        }
        else
        {
            if (keyboard.IsKeyPressed(Keys.Up))
            {
                _map.TryMovePlayer(Direction.Up);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                _map.TryMovePlayer(Direction.Down);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Left))
            {
                _map.TryMovePlayer(Direction.Left);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Right))
            {
                _map.TryMovePlayer(Direction.Right);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad8))
            {
                _map.TryMovePlayer(Direction.Up);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad2))
            {
                _map.TryMovePlayer(Direction.Down);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad4))
            {
                _map.TryMovePlayer(Direction.Left);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad6))
            {
                _map.TryMovePlayer(Direction.Right);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad7))
            {
                _map.TryMovePlayer(Direction.UpLeft);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad9))
            {
                _map.TryMovePlayer(Direction.UpRight);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad1))
            {
                _map.TryMovePlayer(Direction.DownLeft);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad3))
            {
                _map.TryMovePlayer(Direction.DownRight);
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.NumPad5))
            {
                handled = true;
            }
            else if (keyboard.IsKeyPressed(Keys.I))
            {
                ShowUseInventory();
            }
            else if (keyboard.IsKeyPressed(Keys.C))
            {
                Map.Instance.Player.CastSpell(1);
            }
        }

        if (handled) {
            Tick();
        }

        return handled;
    }

    private void ShowUseInventory()
    {
        List<Item> items = Map.Instance.Player.Inventory;
        if (items.Count == 0)
        {
            ShowMessage("You have no items in your inventory.");
            return;
        }

        string inventory = "Inventory: ";
        for (int i = 0; i < items.Count; i++)
        {
            if (i == items.Count - 1)
                inventory += $"{i + 1}: {items[i].Id}";
            else
                inventory += $"{i + 1}: {items[i].Id}, ";
        }

        _showingUseInventory = true;

        _inventoryScreen.Clear();
        _inventoryScreen.Print(0, 0, "(Press number keys to use items, press esc to exit inventory)", Color.DarkOrange,
            Color.Black);
        _inventoryScreen.Print(0, 1, inventory, Color.White, Color.Black);
    }

    public void ShowMessage(string message)
    {
        _messages.Add(message);
    }

    public void ShowLineOfSightTargeting()
    {
        _targetingActive = true;
        _targetingScreen.Clear();
        _targetingCursorScreen.Clear();
        _targetingCursorScreen.Print(0, 0, "Targeting", Color.White, Color.Black);
        _targetingCursorScreen.Print(Map.Instance.Player.Position.X, Map.Instance.Player.Position.Y, "X", Color.Red);
        _targetingCursorPosition = Map.Instance.Player.Position;

        UpdateLineOfSightTargeting();
    }

    public void UpdateFieldOfView()
    {
        _fieldOfViewScreen.Clear();
        for (int x = 0; x < Game.Instance.ScreenCellsX; x++)
        {
            for (int y = 0; y < Game.Instance.ScreenCellsY; y++)
            {
                _fieldOfViewScreen.Print(x, y, new ColoredString(" ", Color.Transparent, Color.Black));
            }
        }
        RogueSharp.FieldOfView fieldOfView = new RogueSharp.FieldOfView(Map.Instance.RogueSharpMap);
        var fieldOfViewCells = fieldOfView.ComputeFov(Map.Instance.Player.Position.X, Map.Instance.Player.Position.Y, 100, true);
        foreach (ICell cellInterface in fieldOfViewCells)
        {
            Cell cell = (Cell)cellInterface;
            if (cell != null)
                _fieldOfViewScreen.Print(cell.X, cell.Y, new ColoredString(" ", Color.Transparent, new Color(Color.Black, 0.9f)));
        }
        fieldOfViewCells = fieldOfView.ComputeFov(Map.Instance.Player.Position.X, Map.Instance.Player.Position.Y, 50, true);
        foreach (ICell cellInterface in fieldOfViewCells)
        {
            Cell cell = (Cell)cellInterface;
            if (cell != null)
                _fieldOfViewScreen.Print(cell.X, cell.Y, new ColoredString(" ", Color.Transparent, new Color(Color.Black, 0.7f)));
        }
        fieldOfViewCells = fieldOfView.ComputeFov(Map.Instance.Player.Position.X, Map.Instance.Player.Position.Y, 25, true);
        foreach (ICell cellInterface in fieldOfViewCells)
        {
            Cell cell = (Cell)cellInterface;
            if (cell != null)
                _fieldOfViewScreen.Print(cell.X, cell.Y, new ColoredString(" ", Color.Transparent, new Color(Color.Black, 0.5f)));
        }
        fieldOfViewCells = fieldOfView.ComputeFov(Map.Instance.Player.Position.X, Map.Instance.Player.Position.Y, 12, true);
        foreach (ICell cellInterface in fieldOfViewCells)
        {
            Cell cell = (Cell)cellInterface;
            if (cell != null)
                _fieldOfViewScreen.Print(cell.X, cell.Y, new ColoredString(" ", Color.Transparent, new Color(Color.Black, 0.3f)));
        }
        fieldOfViewCells = fieldOfView.ComputeFov(Map.Instance.Player.Position.X, Map.Instance.Player.Position.Y, 6, true);
        foreach (ICell cellInterface in fieldOfViewCells)
        {
            Cell cell = (Cell)cellInterface;
            if (cell != null)
                _fieldOfViewScreen.Print(cell.X, cell.Y, new ColoredString(" ", Color.Transparent, new Color(Color.Black, 0.1f)));
        }
    }

    public void HideTargeting()
    {
        _targetingActive = false;
        _targetingScreen.Clear();
        _targetingCursorScreen.Clear();
    }

    public void UpdateLineOfSightTargeting()
    {
        _targetingCursorScreen.Clear();
        _targetingScreen.Clear();
        _targetingScreen.Print(0, 0, "Targeting", Color.White, Color.Black);
        _targetingScreen.Print(_targetingCursorPosition.X, _targetingCursorPosition.Y, "X", Color.Red);

        List<Point> line = new List<Point>();
        foreach (Point point in Lines.GetBresenhamLine(Map.Instance.Player.Position, _targetingCursorPosition))
        {
            if (point != Map.Instance.Player.Position)
            {
                if (Map.Instance.MapObjectsByPosition[point.X, point.Y].Any(x => x.BlocksMovement))
                {
                    line.Add(point);
                    break;
                }
            }

            line.Add(point);
        }
        
        if (line.Contains(_targetingCursorPosition))
        {
            _targetingCursorScreen.Print(_targetingCursorPosition.X, _targetingCursorPosition.Y, "X", Color.Green);
            foreach (Point point in line)
            {
                _targetingScreen.Print(point.X, point.Y, new ColoredString("X", Color.Transparent, Color.LightGreen));
            }
        }
        else
        {
            _targetingCursorScreen.Print(_targetingCursorPosition.X, _targetingCursorPosition.Y, "X", Color.Red);
            foreach (Point point in line)
            {
                _targetingScreen.Print(point.X, point.Y, new ColoredString("X", Color.Transparent, Color.Red));
            }
        }
    }
}