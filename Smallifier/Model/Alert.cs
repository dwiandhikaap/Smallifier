using System;
using System.Windows;

namespace Smallifier.Model
{
    class Alert
    {
        public static void PopUp(String message)
        {
            MessageBox.Show(message);
        }
        public static void PopUp(Exception exception) {
            var message = exception.Message;
            MessageBox.Show(message);
        }
    }
}
