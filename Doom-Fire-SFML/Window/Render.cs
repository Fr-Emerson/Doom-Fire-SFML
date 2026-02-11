using SFML.System;
using SFML.Window;
using SFML.Graphics;
using Color = SFML.Graphics.Color;

namespace Doom_Fire.Window;

public class Render: RenderWindow
{
    public const int PixelSize = 10;
    public static int NRows;
    public static int NCols;
    private Random _random = new Random();

    private readonly Color[] _palette = new Color[]
    {
        new (7, 7, 7),
        new (31, 7, 7),
        new (47, 15, 7),
        new (71, 15, 7),
        new (87, 23, 7),
        new (103, 31, 7),
        new (119, 31, 7),
        new (143, 39, 7),
        new (159, 47, 7),
        new (175, 63, 7),
        new (191, 71, 7),
        new (199, 71, 7),
        new (223, 79, 7),
        new (223, 87, 7),
        new (223, 87, 7),
        new (215, 95, 7),
        new (215, 95, 7),
        new (215, 103, 15),
        new (207, 111, 15),
        new (207, 119, 15),
        new (207, 127, 15),
        new (207, 135, 23),
        new (199, 135, 23),
        new (199, 143, 23),
        new (199, 151, 31),
        new (191, 159, 31),
        new (191, 159, 31),
        new (191, 167, 39),
        new (191, 167, 39),
        new (191, 175, 47),
        new (183, 175, 47),
        new (183, 183, 47),
        new (183, 183, 55),
        new (207, 207, 111),
        new (223, 223, 159),
        new (239, 239, 199),
        new (255, 255, 255)
    };
    public Render(VideoMode mode, string title) : base(mode, title)
    {
        Closed += (sender, e) => Close();
    }

    public Render(VideoMode mode, string title, Styles style, State state) : base(mode, title, style, state)
    {
        Closed += (sender, e) => Close();
    }

    public Render(VideoMode mode, string title, Styles style, State state, ContextSettings settings) : base(mode, title, style, state, settings)
    {
        Closed += (sender, e) => Close();
    }

    public Render(IntPtr handle) : base(handle)
    {
        Closed += (sender, e) => Close();
    }

    public Render(IntPtr handle, ContextSettings settings) : base(handle, settings)
    {
        Closed += (sender, e) => Close();
    }


    private RectangleShape[] _grid;
    private int[] _gridNumbers;

    private void InitializeGrid()
    {
        NRows = (int)Size.Y / PixelSize;
        NCols = (int)Size.X / PixelSize;
        _grid = new RectangleShape[NRows * NCols];
        _gridNumbers = new int[NRows * NCols];
        for (int i = 0; i < NRows; i++)
        {
            for (int j = 0; j < NCols; j++)
            {
                _grid[i * NCols + j] = new RectangleShape
                {
                    Size = new Vector2f(PixelSize,PixelSize),
                    FillColor = Color.Transparent,
                    Position = new Vector2f(j * PixelSize, i * PixelSize),
                    OutlineColor = Color.Transparent,
                    OutlineThickness = .5f
                };
            }
        }
    }
    private void InitializeFire() {
        for (int j = 0; j < NCols; j++) {
            int lastIndex = (NRows - 1) * NCols + j;
            _gridNumbers[lastIndex] = 36;
        }
    }

    public void DrawGrid() {
        for (int i = 0; i < NRows; i++) {
            for (int j = 0; j < NCols; j++) {
                Draw(_grid[i * NCols + j]);
            }
        }
    }


    private void WriteNumbers()
    {
        for (int i = 0; i < NCols; i++)
        {
            for (int j = 0; j < NRows; j++)
            {
                Console.Write("|"+_gridNumbers[i * NCols + j]+"|");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
    }

    private void CalculateSpreadFire()
    {
        for (int j = 0; j < NCols; j++)
        {
            for (int i = 0; i < NRows; i++)
            {
                int pixelIndex = j + NCols * i;
                UpdateIntensity(pixelIndex);

            }
        }
    }

    private void UpdateIntensity(int currentPixelIndex)
    {
        int decayFactor = _random.Next(0, 3);
        int belowPixellIndex = currentPixelIndex + NCols;
        if (belowPixellIndex>= NCols * NRows)
        {
            return;
        }
        int belowFireIntensity = _gridNumbers[belowPixellIndex];
        int newFireIntensity = belowFireIntensity - decayFactor >= 0 ? belowFireIntensity - decayFactor:0;
        int targetIndex = Math.Clamp(currentPixelIndex - decayFactor, 0, _grid.Length - 1);
        _gridNumbers[targetIndex] = newFireIntensity;
        _grid[targetIndex].FillColor = _palette[newFireIntensity];

    }
    Clock _clock = new ();
    float _updateInterval = 1f / 60f; 
    float _accumulator;
    public void Run()
    {
        
        InitializeGrid();
        InitializeFire();
        
        while (IsOpen)
        {
            float deltaTime = _clock.Restart().AsSeconds();
            _accumulator += deltaTime;
            DispatchEvents();
            Clear(Color.Black);
            if (_accumulator >= _updateInterval) {
                _accumulator -= _updateInterval;
                CalculateSpreadFire();  
            }
        
            DrawGrid();  
            Display();
        }
    }
    
}