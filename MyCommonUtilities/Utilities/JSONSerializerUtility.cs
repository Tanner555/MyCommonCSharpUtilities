using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyCommonUtilities
{
    class JSONSerializerUtility
    {
        #region Getters / Setters

        //private string characterInfoJSON;

        //public static string GetJSON<T>()
        //{
        //    var value = session.GetString(key);

        //    return value == null ? default(T) :
        //        JsonConvert.DeserializeObject<T>(value);
        //}

        //public static void SetJSON<T>(string key, T value)
        //{
        //    session.SetString(key, JsonConvert.SerializeObject(value));
        //}
        #endregion

        #region Testing
        struct MyCharacterInfo
        {
            public int health;
            public int magic;
            public int XP;

            public MyCharacterInfo(int health = 100, int magic = 250, int XP = 1500)
            {
                this.health = health;
                this.magic = magic;
                this.XP = XP;
            }
        }

        public static void TestJSONSerialization()
        {
            var _myCharacter = new MyCharacterInfo(100, 250, 1500);
            Console.WriteLine($"Character Health: {_myCharacter.health}, Magic: {_myCharacter.magic}, XP: {_myCharacter.XP}");
            string _jsonObject = JsonConvert.SerializeObject(_myCharacter);
            Console.WriteLine(_jsonObject);
        }
        #endregion
    }
}
