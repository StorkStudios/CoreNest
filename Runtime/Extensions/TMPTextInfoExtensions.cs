using TMPro;
using UnityEngine;

namespace StorkStudios.CoreNest
{
    public static class TMPTextInfoExtensions
    {
        private const int cornersCount = 4;

        /// <summary>
        /// Writes the new color into the text render buffer
        /// </summary>
        /// <param name="charIndex">Index of the character</param>
        /// <param name="color">Desired color</param>
        /// <param name="pushUpdate">Inform the mesh and the rederer that there was an update. When disabled the user it ought to call <see cref="TMP_Text.UpdateVertexData(TMP_VertexDataUpdateFlags)">UpdateVertexData</see> manually.</param>
        public static void SetCharacterColor(this TMP_TextInfo textInfo, int charIndex, Color32 color, bool pushUpdate)
        {
            int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

            for (int i = 0; i < cornersCount; i++)
            {
                vertexColors[vertexIndex + i] = color;
            }

            if (pushUpdate)
            {
                textInfo.textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }
        }

        /// <summary>
        /// Writes the new color corners into the text render buffer
        /// </summary>
        /// <param name="charIndex">Index of the character</param>
        /// <param name="colorCorners">An array of length 4 of desired color corners</param>
        /// <param name="pushUpdate">Inform the mesh and the rederer that there was an update. When disabled the user it ought to call <see cref="TMP_Text.UpdateVertexData(TMP_VertexDataUpdateFlags)">UpdateVertexData</see> manually.</param>
        public static void SetCharacterColor(this TMP_TextInfo textInfo, int charIndex, Color32[] colorCorners, bool pushUpdate)
        {
            if (colorCorners.Length != cornersCount)
            {
                throw new System.Exception($"Expected an array of length {4} (array of length {colorCorners.Length} given).");
            }

            int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

            for (int i = 0; i < cornersCount; i++)
            {
                vertexColors[vertexIndex + i] = colorCorners[i];
            }

            if (pushUpdate)
            {
                textInfo.textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }
        }

        /// <summary>
        /// Gets the color corners from the text render buffer
        /// </summary>
        /// <param name="charIndex">Index of the character</param>
        /// <param name="colorCorners">Output array of length 4</param>
        public static void GetCharacterColorCorners(this TMP_TextInfo textInfo, int charIndex, Color32[] colorCorners)
        {
            if (colorCorners.Length != cornersCount)
            {
                throw new System.Exception($"Expected an array of length {4} (array of length {colorCorners.Length} given).");
            }

            int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

            for (int i = 0; i < cornersCount; i++)
            {
                colorCorners[i] = vertexColors[vertexIndex + i];
            }
        }
    }
}