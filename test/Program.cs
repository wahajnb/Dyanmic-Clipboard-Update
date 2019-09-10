using System;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Input;

namespace EventHook.ConsoleApp.Example
{
    internal class Program
    {
        [STAThreadAttribute]
        private static void Main(string[] args)
        {
            var eventHookFactory = new EventHookFactory();
			
			//Provide text that needs to be dynamically added to the clipboard
            var text = "wait-time 2 secs music goto step 15 service-hours not-in V2 goto step 9 staffed-agents 2298 > 0 goto step 12 staffed-agents 2299 > 0 goto step 9 staffed-agents 1st = 0 route-to number V1 n unconditionally stop announcement 2840102 disconnect 2840107 stop announcement 2840103 VECTOR disconnect 2840107 stop announcement 2840104 disconnect 2840107 stop";
            var lst = text.Split(' ');
			
			//Prints first word in text to Console
            Console.WriteLine(lst[0]);
            var i = 0;
			
			//Initialises MouseWatcher
            var mouseWatcher = eventHookFactory.GetMouseWatcher();
            mouseWatcher.Start();
            mouseWatcher.OnMouseInput += (s, e) =>
            {
                if (e.Message.ToString() != "WM_MOUSEMOVE")
                {
                    Console.WriteLine("Mouse event {0} at point {1},{2}", e.Message.ToString(), e.Point.x, e.Point.y);
                }
            };
			
			//Initialises Clipboard Watcher to check for clipboard update events
            var clipboardWatcher = eventHookFactory.GetClipboardWatcher();
            clipboardWatcher.Start();
            clipboardWatcher.OnClipboardModified += (s, e) =>
            {
                Console.WriteLine("Clipboard updated with data '{0}' of format {1}", e.Data,
                    e.DataFormat.ToString());
 
            };

            //var applicationWatcher = eventHookFactory.GetApplicationWatcher();
            //applicationWatcher.Start();
            //applicationWatcher.OnApplicationWindowChange += (s, e) =>
            //{
            //    Console.WriteLine("Application window of '{0}' with the title '{1}' was {2}",
            //        e.ApplicationData.AppName, e.ApplicationData.AppTitle, e.Event);
            //};

			//Initialises Keyboard Watcher to check key press events
            var keyboardWatcher = eventHookFactory.GetKeyboardWatcher();
            keyboardWatcher.Start();
            keyboardWatcher.OnKeyInput += (s, e) =>
            {
				//Updates Clipboard with the next word in the list when "TAB" is pressed
                if (e.KeyData.Keyname == "Tab" && e.KeyData.EventType.ToString() == "down")
                {
                    Console.WriteLine("Key {0} event of key {1}", e.KeyData.EventType, e.KeyData.Keyname);

                    //Creates new thread to handle the clipboard update
                    Thread thread = new Thread(() => Clipboard.SetText(lst[i]));
                    thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                    thread.Start();
                    thread.Join();
                    i++;
                    i = i % (lst.Length);
                    //Console.WriteLine(lst[i]);
                }

                //Console.WriteLine("Key {0} event of key {1}", e.KeyData.EventType, e.KeyData.Keyname);
            };

            Console.Read();
            //mouseWatcher.Stop();
            clipboardWatcher.Stop();
            //applicationWatcher.Stop();
            keyboardWatcher.Stop();


            eventHookFactory.Dispose();
        }
    }
}