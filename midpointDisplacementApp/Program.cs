using System.Numerics;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;



public class MidpointDisplacement
{
    //A program that generates a random, rough terrain using the midpoint displacement
    //algorithm

    private float randomToPlusMinusOffsetValue()
    {
        Random random = new Random();
        double randomValue = random.Next(0, 2);


        const double plusMinusOffsetRange = 2;
        const double plusMinusOffset = -1;
        //0-1 * 2 returns 0-2, then offset it by -1 to get -1 - 1
        double offsetValue = randomValue * plusMinusOffsetRange + plusMinusOffset;

        return (float)offsetValue;



    }

    private (float x, float y) displacePoint((float x, float y) initialCoordinate, float displacementLimit)
    {

        float displacedY = initialCoordinate.y + displacementLimit * randomToPlusMinusOffsetValue();

        return (initialCoordinate.x, displacedY);

    }

    private (float x, float y) createDisplacedPointFromInterpolatingTwoGivenPoints((float x, float y) point1, (float x, float y) point2, float displacementAmount)
    {
        float differenceBetweenX = point1.x - point2.x;
        float differenceBetweenY = point1.y - point2.y;

        float pointWithAveragedX = differenceBetweenX / 2 + point2.x;
        float pointWithAveragedY = differenceBetweenY / 2 + point2.y;

        (float x, float y) initialCoordinate = (pointWithAveragedX, pointWithAveragedY);

        (float x, float y) displacedCoordinate = displacePoint(initialCoordinate, displacementAmount);

        return displacedCoordinate;

    }

    public List<(float x, float y)> initialiseAndLoopMidpointDisplacementAlgorithm(List<(float, float)> pointList, int iterations, float displacementAmount, float decayPower)
    {


        //= 1/2^decayPower
        float displacementReduction = 1 / (float)Math.Pow(2.0f, decayPower);



        for (int z = 0; z < iterations; z++)
        {

            for (int i = 1; i < pointList.Count; i += 2)
            {
                (float x, float y) newPoint = createDisplacedPointFromInterpolatingTwoGivenPoints(pointList[i - 1], pointList[i], displacementAmount);
                pointList.Insert(i, newPoint);
            }
            displacementAmount *= displacementReduction;

        }

        Console.WriteLine("List has " + pointList.Count + " points");
        return pointList;
    }

}




public class CustomWindow : Form
{

    WorldContext worldContext;

    List<(float x, float y)> pointList = new List<(float x, float y)>();

    public CustomWindow(WorldContext wc)
    {
        Size = new Size(960, 540);
        DoubleBuffered = true;
        worldContext = wc;
        this.MouseDown += new System.Windows.Forms.MouseEventHandler(mouseDown);
        this.MouseMove += new System.Windows.Forms.MouseEventHandler(mouseMove);
        this.MouseUp += new System.Windows.Forms.MouseEventHandler(mouseUp);
    }

    (float x, float y) mouseInitialLocation;
    bool leftMouseClicked = false;
    public void mouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            mouseInitialLocation = (e.X, e.Y);
            leftMouseClicked = true;

            
        }
    }

    public void mouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            leftMouseClicked = false;
        }
    }

    public void mouseMove(object sender, MouseEventArgs e)
    {
        if (leftMouseClicked)
        {
            worldContext.updateScreenspaceLocation((e.X - mouseInitialLocation.x, e.Y - mouseInitialLocation.y));
            mouseInitialLocation = (e.X, e.Y);    
        }
    }


    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics graphics = e.Graphics;
        Pen drawingPen = new Pen(Color.Black, 3f);
        for (int i = 1; i < worldContext.terrainPointList.Count; i++)
        {
            int p1XWithOffset = (int)worldContext.terrainPointList[i - 1].x + (int)worldContext.screenSpaceOffset.x;
            int p1YWithOffset = (int)worldContext.terrainPointList[i - 1].y + (int)worldContext.screenSpaceOffset.y;

            int p2XWithOffset = (int)worldContext.terrainPointList[i].x + (int)worldContext.screenSpaceOffset.x;
            int p2YWithOffset = (int)worldContext.terrainPointList[i].y + (int)worldContext.screenSpaceOffset.y;
            //Convert touples to Points
            Point p1 = new Point(p1XWithOffset, p1YWithOffset);
            Point p2 = new Point(p2XWithOffset, p2YWithOffset);

            graphics.DrawLine(drawingPen, p1, p2);
        }
        
    }
    

 }

public class WorldContext
{
    public List<(float x, float y)> terrainPointList = new List<(float x, float y)>();

    public (float x, float y) screenSpaceOffset = (0f, 0f);

    public void generateTerrain(List<(float, float)> initialPointList, int iterations, float displacementAmount, float decayPower)
    {
        MidpointDisplacement md = new MidpointDisplacement();

        terrainPointList = md.initialiseAndLoopMidpointDisplacementAlgorithm(initialPointList, iterations, displacementAmount, decayPower);
    }

    public void updateScreenspaceLocation((float x, float y) offset)
    {
        screenSpaceOffset = (screenSpaceOffset.x + offset.x, screenSpaceOffset.y + offset.y);
          
    }

}

public class ProgramController
{
    WorldContext worldContext;
    Timer timer;
    CustomWindow window;

    public void InitialiseWorld()
    {
        worldContext = new WorldContext();

        List<(float, float)> initialTerrainPoints = new List<(float, float)>();

        initialTerrainPoints.Add((0, 200f));
        
        initialTerrainPoints.Add((2000f, 200f));
        const int iterations = 9;
        const float initialDisplacement = 200f;
        const float decayPower = 1.4f;

        worldContext.generateTerrain(initialTerrainPoints, iterations, initialDisplacement, decayPower);
        int updateInterval = 10;


        timer = new Timer();
        timer.Interval = updateInterval;
        timer.Tick += onTimerTick;
        timer.Start();

        window = new CustomWindow(worldContext);


        window.ShowDialog();

    }


    public void onTimerTick(object sender, EventArgs e)
    {
        //Update All Objects require tick updates
        window.Refresh();
    }

}

public class Initialiser
{
    public static void Main(string[] args)
    {
        ProgramController pc = new ProgramController();
        pc.InitialiseWorld();    
    }
}
