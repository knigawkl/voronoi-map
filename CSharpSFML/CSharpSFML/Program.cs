using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSharpSFML;
using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SFML_Test
{
    static class Program
    {

        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static void Main()
        {
            // Create the main window
            RenderWindow app = new RenderWindow(new VideoMode(1920, 1080), "SFML Works!");
            app.Closed += new EventHandler(OnClose);


            Color windowColor = new Color(0, 0, 0);

            // create an empty shape
            ConvexShape convex = new ConvexShape();

            // resize it to 5 points
            convex.SetPointCount(8);

            // define the points
            convex.SetPoint(0, new Vector2f(106, 193));
            convex.SetPoint(1, new Vector2f(149, 681));
            convex.SetPoint(2, new Vector2f(567, 938));
            convex.SetPoint(3, new Vector2f(1027, 990));
            convex.SetPoint(4, new Vector2f(1138, 737));
            convex.SetPoint(5, new Vector2f(1022, 120));
            convex.SetPoint(6, new Vector2f(649, 103));
            convex.SetPoint(7, new Vector2f(457, 53));

            convex.FillColor = new Color(255, 0, 0);
            // Start the game loop
            while (app.IsOpen)
            {
                
            /*if (event.type == sf::Event::Closed)
				mainWindow.close();
            }*/
                // Process events
                app.DispatchEvents();
                // Clear screen
                app.Clear(windowColor);

                // Update the window
                app.Draw(convex);
                app.Display();
                Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            } //End game loop
        } //End Main()
    } //End Program
}
