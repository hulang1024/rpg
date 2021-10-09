using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Characters
{
    public class CharacterStyleManager
    {
        private readonly string file_path = $"{OS.GetUserDataDir()}/character_styles.json";

        public List<CharacterStyleInfo> AllStyles { get; private set; }

        private bool loaded = false;

        public int LoadAll(bool force = true)
        {
            if (loaded && !force) return 0;
            System.IO.StreamReader reader = null;
            try
            {
                reader = System.IO.File.OpenText(file_path);
                string json = reader.ReadToEnd();
                AllStyles = JsonConvert.DeserializeObject<List<CharacterStyleInfo>>(json);
                loaded = true;
                return 0;
            }
            catch (System.IO.FileNotFoundException)
            {
                AllStyles = new List<CharacterStyleInfo>();
                return 1;
            }
            finally
            {
                reader?.Dispose();
            }
        }

        public CharacterStyleInfo FindById(string id)
        {
            LoadAll(false);
            return AllStyles.Find(item => item.Id == id);
        }

        public List<CharacterStyleInfo> Search(CharacterStyleSearchParam param)
        {
            LoadAll();
            var items = AllStyles;
            if (param.Gender != null)
            {
                items = items.FindAll(item => item.Gender == param.Gender);
            }
            if (param.IsKid != null)
            {
                items = items.FindAll(item => item.Styles.IsKid == param.IsKid);
            }
            if (!string.IsNullOrEmpty(param.Tags))
            {
                items = items.FindAll(item => item.Tags.IndexOf(param.Tags) != -1);
            }
            return items;
        }
    
        public int Add(CharacterStyleInfo styleInfo)
        {
            LoadAll();
            if (AllStyles.Exists(ch => ch.Equals(styleInfo)))
            {
                return 2;
            }
            if (string.IsNullOrEmpty(styleInfo.Id))
            {
                styleInfo.Id = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString("X");
            }
            else if (styleInfo.Id.IndexOf(",") != -1)
            {
                return 4;
            }

            if (AllStyles.Exists(ch => ch.Id == styleInfo.Id))
            {
                return 3;
            }
            AllStyles.Add(styleInfo);

            return save();
        }

        public int Update(string id, CharacterStyleInfo styleInfo)
        {
            LoadAll();
            var oldStyleInfo = AllStyles.Find(ch => ch.Id == id);
            if (oldStyleInfo == null)
                return 1;
            if (id != styleInfo.Id)
            {
                oldStyleInfo.OldIds = string.IsNullOrEmpty(oldStyleInfo.OldIds)
                    ? oldStyleInfo.Id
                    : oldStyleInfo.OldIds + "," + oldStyleInfo.Id;
                oldStyleInfo.Id = styleInfo.Id;
            }
            oldStyleInfo.Styles = styleInfo.Styles;
            oldStyleInfo.Gender = styleInfo.Gender;
            oldStyleInfo.Tags = styleInfo.Tags;
            oldStyleInfo.Remark = styleInfo.Remark;

            return save();
        }

        public int Delete(string id)
        {
            LoadAll();
            if (AllStyles.RemoveAll(t => t.Id == id) > 0)
                return save();
            return 1;
        }

        private int save()
        {
            var jsonifySetting = new JsonSerializerSettings();
            jsonifySetting.NullValueHandling = NullValueHandling.Ignore;
            jsonifySetting.Formatting = Formatting.Indented;

            var json = JsonConvert.SerializeObject(AllStyles, jsonifySetting);
            System.IO.File.WriteAllText(file_path, json);

            return 0;
        }
    }
}