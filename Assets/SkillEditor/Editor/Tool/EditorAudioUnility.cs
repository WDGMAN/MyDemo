using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class EditorAudioUnility
{
   private static MethodInfo playClipMethodInfo;
   private static MethodInfo stopAllClipMethodInfo;
   static EditorAudioUnility()
   {
       
    Assembly editorAssembly= typeof(UnityEditor.AudioImporter).Assembly;
    Type utilClassType = editorAssembly.GetType("UnityEditor.AudioUtil");

    playClipMethodInfo = utilClassType.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public, null,
        new Type[]{typeof(AudioClip), typeof(int), typeof(bool)},null);
    
    stopAllClipMethodInfo = utilClassType.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public);
   }

   /// <summary>
   /// 播放音效
   /// </summary>
   /// <param name="clip"></param>
   /// <param name="start"></param>
   public static void PlayAudio(AudioClip clip, float start)
   {
       playClipMethodInfo.Invoke(null, new object[]{clip, (int)(start * clip.frequency), false});
   }

   public static void StopAllAudios()
   {

       stopAllClipMethodInfo.Invoke(null, null);
   }
}
