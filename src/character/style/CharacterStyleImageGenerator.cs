using Godot;
using System;
using System.Threading.Tasks;

namespace Characters
{
    public class CharacterStyleImageGenerator
    {
        private const string BASE_PATH = "user://res/character";

        public static async Task<ImageTexture> GenerateImageTexture(CharacterStyleSpecifics specifics)
        {
            var image = await GenerateImage(specifics);
            return image == null ? null : CharacterStyleTexture.From(image);
        }

        public static Task<Image> GenerateImage(CharacterStyleSpecifics specifics)
        {
            Action<Image, string, CharacterStyleComponentType, string, bool> LoadAddComponent =
                (dest, dirName, componentType, typeName, isAdd) =>
            {
                var frameSizeDirName = GetFrameSizeName(specifics.FrameSize);

                var isKid = specifics.IsKid
                    && Array.Exists(new CharacterStyleComponentType[]
                        {
                            CharacterStyleComponentType.Body,
                            CharacterStyleComponentType.Eyes,
                            CharacterStyleComponentType.Hairstyle,
                            CharacterStyleComponentType.Outfit
                        }, t => t == componentType);
                var dirNameSuffix = isKid ? "_kids" : "";
                var image = isAdd ? new Image() : dest;
                image.Load($"{BASE_PATH}/{dirName}{dirNameSuffix}/{frameSizeDirName}/{typeName}.png");
                if (isAdd)
                {
                    dest.BlendRect(image, new Rect2(Vector2.Zero, image.GetSize()), Vector2.Zero);
                }
            };

            var task = new Task<Image>(() =>
            {
                if (string.IsNullOrEmpty(specifics.Body)) return null;

                var characterImage = new Image();
                LoadAddComponent.Invoke(characterImage, "Bodies",
                    CharacterStyleComponentType.Body, specifics.Body, false);
                if (!string.IsNullOrEmpty(specifics.Eyes))
                    LoadAddComponent.Invoke(characterImage, "Eyes",
                        CharacterStyleComponentType.Eyes, specifics.Eyes, true);
                if (!string.IsNullOrEmpty(specifics.Outfit))
                    LoadAddComponent.Invoke(characterImage, "Outfits",
                        CharacterStyleComponentType.Outfit, specifics.Outfit, true);
                if (!string.IsNullOrEmpty(specifics.Hairstyle))
                    LoadAddComponent.Invoke(characterImage, "Hairstyles",
                        CharacterStyleComponentType.Hairstyle, specifics.Hairstyle, true);
                if (!string.IsNullOrEmpty(specifics.Smartphones))
                    LoadAddComponent.Invoke(characterImage, "Smartphones",
                        CharacterStyleComponentType.Smartphones, specifics.Smartphones, true);
                if (!string.IsNullOrEmpty(specifics.Accessories))
                    LoadAddComponent.Invoke(characterImage, "Accessories",
                        CharacterStyleComponentType.Accessories, specifics.Accessories, true);
                return characterImage;
            });
            task.Start();
            return task;
        }

        public static String GetFrameSizeName(CharacterFrameSize sizeEnum)
        {
            int size = new int[]{ 16, 32, 48 }[(int)sizeEnum];
            return $"{size}x{size}";
        }
    }
}