using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using NX.Hooks;

namespace NX_Overmind.Actions
{
    /// <summary>
    /// Helper class for parsing HookEvent objects
    /// </summary>
    public class HookEventHelper
    {
        /// <summary>
        /// Get modifier string consisting of Alt, Ctrl, Shift and Caps Lock
        /// </summary>
        /// <param name="ha">HookEventArgs object</param>
        /// <returns>Modifier string</returns>
        public static string GetModifierString(HookEventArgs ha)
        {
            string str = "";
            if (ha.Alt)
                str += "Alt + ";
            if (ha.Control)
                str += "Ctrl + ";
            if (ha.Shift)
                str += "Shift + ";
            if (ha.CapsLock)
                str += "CapsLock + ";
            return str.TrimEnd(new char[] { ' ', '+' });
        }

        /// <summary>
        /// Converts a list of hook events to string
        /// </summary>
        /// <param name="args">List of HookEventArgs objects</param>
        /// <returns>String representing the list of events</returns>
        public static string HookEventsToString(HookEventArgs[] args)
        {
            string eventStr = "";
            foreach (HookEventArgs arg in args)
            {
                string str = "";
                if ((arg as MouseHookEventArgsEx) != null)
                {
                    MouseHookEventArgsEx e = arg as MouseHookEventArgsEx;
                    switch (e.Type)
                    {
                        case HookEventCodec.EventType.MouseDown:
                            str = String.Format("[Mouse Down: {0} | {1},{2} : {3}]", e.Button.ToString(), e.Location.X, e.Location.Y, HookEventHelper.GetModifierString(e));
                            break;
                        case HookEventCodec.EventType.MouseClick:
                            str = String.Format("[Mouse Click: {0}#{1} | {2},{3} : {4}]", e.Button.ToString(), e.ClickCount, e.Location.X, e.Location.Y, HookEventHelper.GetModifierString(e));
                            break;
                        case HookEventCodec.EventType.MouseDoubleClick:
                            str = String.Format("[Double Click: {0} | {1},{2} : {3}]", e.Button.ToString(), e.Location.X, e.Location.Y, HookEventHelper.GetModifierString(e));
                            break;
                        case HookEventCodec.EventType.MouseUp:
                            str = String.Format("[Mouse Up: {0} | {1},{2} : {3}]", e.Button.ToString(), e.Location.X, e.Location.Y, HookEventHelper.GetModifierString(e));
                            break;
                        case HookEventCodec.EventType.MouseMove:
                            str = String.Format("[Mouse Move | {0},{1} : {2}]", e.Location.X, e.Location.Y, HookEventHelper.GetModifierString(e));
                            break;
                        case HookEventCodec.EventType.MouseWheel:
                            str = String.Format("[Mouse Scroll: {0} | {1},{2} : {3}]", e.ScrollAmount, e.Location.X, e.Location.Y, HookEventHelper.GetModifierString(e));
                            break;
                    }
                }
                else if ((arg as KeyHookEventArgsEx) != null)
                {
                    KeyHookEventArgsEx e = arg as KeyHookEventArgsEx;
                    List<string> pKeys = new List<string>();
                    foreach (byte b in e.PressedKeys)
                        if (b != 0)
                            pKeys.Add(((Keys)b).ToString());

                    switch (e.Type)
                    {
                        case HookEventCodec.EventType.KeyDown:
                            str = String.Format("[Down: {0} : {1}]", String.Join(" + ", pKeys.ToArray()), HookEventHelper.GetModifierString(e));
                            break;
                        case HookEventCodec.EventType.KeyPress:
                            str = String.Format("[{0} : {1}]", String.Join(" + ", pKeys.ToArray()), HookEventHelper.GetModifierString(e));
                            break;
                        case HookEventCodec.EventType.KeyUp:
                            str = String.Format("[Up: {0} : {1}]", String.Join(" + ", pKeys.ToArray()), HookEventHelper.GetModifierString(e));
                            break;
                    }
                    if (e.KeyChar != '\0')
                        str += e.KeyChar;
                }
                str.Trim();
                if (str != "")
                    eventStr += str + Environment.NewLine;
            }
            return eventStr.Trim();
        }

        /// <summary>
        /// Decodes a stream for a list of events
        /// </summary>
        /// <param name="ls">Event log stream</param>
        /// <returns>List of events</returns>
        public static HookEventArgs[] StreamToHookEvents(Stream ls)
        {
            List<HookEventArgs> hookEvents = new List<HookEventArgs>();

            MemoryStream ms = ls as MemoryStream;
            HookEventCodec.EventClass ec;
            HookEventCodec.EventType et;
            HookEventArgs ha;
            byte[] pressedKeys;
            char pressedChar;
            int offset = 0;
            while (HookEventCodec.GetDecodedData(offset, ms.ToArray(), out ec, out et, out ha, out pressedKeys, out pressedChar))
            {
                if (ec == HookEventCodec.EventClass.MouseEvent)
                {
                    ha = new MouseHookEventArgsEx(ha as MouseHookEventArgs, et);
                    offset += (int)HookEventCodec.Offsets.MouseLogOffset;
                }
                else
                {
                    KeyHookEventArgs e = ha as KeyHookEventArgs;
                    List<System.Windows.Forms.Keys> pKeys = new List<System.Windows.Forms.Keys>();
                    foreach (byte b in pressedKeys)
                        if (b != 0)
                            pKeys.Add((System.Windows.Forms.Keys)b);

                    offset += (int)HookEventCodec.Offsets.KeyLogOffset;
                    ha = new KeyHookEventArgsEx(e, et, pKeys, pressedChar);
                }
                hookEvents.Add(ha);
            }
            return hookEvents.ToArray();
        }

        /// <summary>
        /// Converts log stream to string representation
        /// </summary>
        /// <param name="ls">Event log stream</param>
        /// <returns>String representation</returns>
        public static string StreamToString(Stream ls)
        {
            return HookEventHelper.HookEventsToString(StreamToHookEvents(ls));
        }
    }
}
