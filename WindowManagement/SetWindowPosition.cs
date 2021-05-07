using UnityEngine;
using System.Collections;
using System ;
using System.Runtime.InteropServices;


namespace JashProjectionMappingSystem.Helpers
{
    public class SetWindowPosition : MonoBehaviour
    {

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, uint wFlags);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(System.String className, System.String windowName);
        //Import window changing function
        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        //Gets window attributes
        [DllImport("USER32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;
        private const int GWL_STYLE = -16;              //hex constant for style changing
        private const int WS_BORDER = 0x00800000;       //window with border
        private const int WS_SYSMENU = 0x00080000;      //window with no borders etc.
        private const int WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox

        //assorted constants needed
        public static int WS_CHILD = 0x40000000; //child window
        public static int WS_DLGFRAME = 0x00400000; //window with double border but no title
        public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar

        public static void SetPosition(int x, int y, int resX = 0, int resY = 0)
        {
            Debug.Log(string.Format("SetPosition:: X:{0}, Y{1}, Width:{2}, Height:{3}", x, y, resX, resY));
            SetWindowPos(GetActiveWindow(), 0, x, y, resX, resY, (uint)(resX * resY == 0 ? 1 : 0));
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        public static string windowTitle = "ProjectionFusion";

        public bool doGUI = false;

        public GameObject activeObject;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);


        // Use this for initialization
        void Awake()
        {
            if (Application.isEditor)
            {
                Invoke("ActiveObject", 2F);
                SetWindowEx(windowPosX, windowPosY, windowWidth, windowHeight);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                doGUI = !doGUI;
            }
        }

        void ActiveObject()
        {
            if (activeObject != null)
                activeObject.SetActive(true);
        }

        public static void SetWindowEx(int posX, int posY, int resW, int resH)
        {
            IntPtr windowFound = GetActiveWindow();
            if (windowFound != IntPtr.Zero)
            {
                SetWindowPos(windowFound, -1, posX, posY, resW, resH, 0);// SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                SetWindowPos(windowFound, -1, posX, posY, resW, resH, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                int style = GetWindowLong(windowFound, GWL_STYLE);
                SetWindowLong(windowFound, GWL_STYLE, (style & ~WS_CAPTION));
            }
            else
            {
                Debug.LogError("Active Window: Not Found!");
            }
        }

        public int windowPosX = 0, windowPosY = 0, windowWidth = 0, windowHeight = 0;

        void OnGUI()
        {
            if (!doGUI)
                return;

            GUILayout.BeginArea(new Rect(20, 20, 600, 200));

            windowTitle = GUILayout.TextField(windowTitle, GUILayout.Width(265));
            GUILayout.BeginHorizontal();
            GUILayout.Box("Set Window Position X: ", GUILayout.Width(150));
            int.TryParse(GUILayout.TextField(windowPosX.ToString(), GUILayout.Width(40)), out windowPosX);
            GUILayout.Box("Y: ", GUILayout.Width(20));
            int.TryParse(GUILayout.TextField(windowPosY.ToString(), GUILayout.Width(40)), out windowPosY);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Set", GUILayout.Width(265)))
            {
                Debug.Log("SetFlag: " + ((int)Camera.main.pixelWidth * (int)Camera.main.pixelHeight == 0 ? 1 : 0));
                SetPosition(windowPosX, windowPosY, (int)Camera.main.pixelWidth, (int)Camera.main.pixelHeight);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Box("Set Window Width: ", GUILayout.Width(125));
            int.TryParse(GUILayout.TextField(windowWidth.ToString(), GUILayout.Width(40)), out windowWidth);
            GUILayout.Box("Height: ", GUILayout.Width(45));
            int.TryParse(GUILayout.TextField(windowHeight.ToString(), GUILayout.Width(40)), out windowHeight);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Set", GUILayout.Width(265)))
            {
                SetPosition(windowPosX, windowPosY, windowWidth, windowHeight);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Hide", GUILayout.Width(265)))
            {
                Destroy(this);
            }

            if (GUILayout.Button("Quit Application", GUILayout.Width(265)))
            {
                Application.Quit();
            }

            GUILayout.EndArea();
        }
    }
}