using MU.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using WebZen.Network;
using BlubLib;

namespace MU.Network
{
    public class VersionSelector
    {

        private Dictionary<ushort, Type> _active;
        private Dictionary<ushort, Type> _types = new Dictionary<ushort, Type>();
        private Dictionary<Type, ushort> _opCodeLookUp = new Dictionary<Type, ushort>();

        private static VersionSelector s_instance;

        private VersionSelector() { }

        public static void Initialize()
        {
            if (s_instance == null)
                s_instance = new VersionSelector();
        }

        /// <summary>
        /// Associate a class with a server version
        /// </summary>
        /// <typeparam name="_T">Class Type of message</typeparam>
        /// <param name="season">server version</param>
        /// <param name="opCode">The message operation code</param>

        /// <summary>
        /// Create a message version matched with current server version
        /// </summary>
        /// <typeparam name="_T">Class Type of message</typeparam>
        /// <param name="args">Constructor Args</param>
        /// <returns>Instance of message</returns>
        public static object CreateMessage<_T>(params object[] args)
            where _T : new()
        {
          

            try
            {
                var result = s_instance._opCodeLookUp[typeof(_T)];
                if (s_instance._active?.ContainsKey(result) ?? false)
                {
                    var type = s_instance._active[result];
                    return Activator.CreateInstance(type, args);
                }

                var subType = s_instance._types.Where((e) => e.Key == result).OrderByDescending(x => x.Key).First().Value;


                            //.OrderByDescending(x => x.Key)
                            //.First()
                            //.Value[result];

                return Activator.CreateInstance(subType, args);
            }
            catch (Exception)
            {
                return Activator.CreateInstance(typeof(_T), args);
            }
        }
    }
}
