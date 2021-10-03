using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Characters
{
    public class PremadeCharacterManager
    {
        private readonly string file_path = $"{OS.GetUserDataDir()}/premade_characters.json";
        private List<PremadeCharacterInfo> allPremadeCharacters;
        public List<PremadeCharacterInfo> AllPremadeCharacters
        {
            get { return allPremadeCharacters; }
        }

        private bool loaded = false;

        public int LoadAll(bool force = true)
        {
            if (loaded && !force) return 0;
            System.IO.StreamReader reader = null;
            try
            {
                reader = System.IO.File.OpenText(file_path);
                string json = reader.ReadToEnd();
                allPremadeCharacters = JsonConvert.DeserializeObject<List<PremadeCharacterInfo>>(json);
                loaded = true;
                return 0;
            }
            catch (System.IO.FileNotFoundException)
            {
                allPremadeCharacters = new List<PremadeCharacterInfo>();
                return 1;
            }
            finally
            {
                reader?.Dispose();
            }
        }

        public PremadeCharacterInfo FindById(string id)
        {
            LoadAll(false);
            return allPremadeCharacters.Find(item => item.Id == id);
        }

        public List<PremadeCharacterInfo> Search(PremadeSearchParam param)
        {
            LoadAll();
            var items = allPremadeCharacters;
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
    
        public int Add(PremadeCharacterInfo premade)
        {
            LoadAll();
            if (allPremadeCharacters.Exists(ch => ch.Equals(premade)))
            {
                return 2;
            }
            if (string.IsNullOrEmpty(premade.Id))
            {
                premade.Id = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString("X");
            }
            else if (premade.Id.IndexOf(",") != -1)
            {
                return 4;
            }

            if (allPremadeCharacters.Exists(ch => ch.Id == premade.Id))
            {
                return 3;
            }
            allPremadeCharacters.Add(premade);

            return Save();
        }

        public int Update(string id, PremadeCharacterInfo premade)
        {
            LoadAll();
            var oldPremade = allPremadeCharacters.Find(ch => ch.Id == id);
            if (oldPremade == null)
                return 1;
            if (id != premade.Id)
            {
                oldPremade.OldIds = string.IsNullOrEmpty(oldPremade.OldIds)
                    ? oldPremade.Id
                    : oldPremade.OldIds + "," + oldPremade.Id;
                oldPremade.Id = premade.Id;
            }
            oldPremade.Styles = premade.Styles;
            oldPremade.Gender = premade.Gender;
            oldPremade.Tags = premade.Tags;
            oldPremade.Remark = premade.Remark;

            return Save();
        }

        public int Delete(string id)
        {
            LoadAll();
            if (allPremadeCharacters.RemoveAll(t => t.Id == id) > 0)
                return Save();
            return 1;
        }

        private int Save()
        {
            var jsonSetting = new JsonSerializerSettings();
            jsonSetting.NullValueHandling = NullValueHandling.Ignore;
            jsonSetting.Formatting = Formatting.Indented;
            var json = JsonConvert.SerializeObject(allPremadeCharacters, jsonSetting);
            System.IO.File.WriteAllText(file_path, json);
            return 0;
        }
    }
}