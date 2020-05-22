using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JHchoi;
using JHchoi.Common;
using JHchoi.Models;

public enum eUIAniType
{

}

public class Util : MonoSingleton<Util>
{
    //-----------------------------------------------------------------------
    public void PlayAnimator(eUIAniType type, GameObject obj, string LayerType = "UIPlay")
    {
        Animator ani = obj.GetComponent<Animator>();
        if (ani == null)
            ani = obj.AddComponent<Animator>();

        if (ani.runtimeAnimatorController == null)
        {
            string loadPath = GetAnimatorClip(type);
            RuntimeAnimatorController clip = Resources.Load(loadPath, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
            if (clip == null)
                return;

            ani.runtimeAnimatorController = clip;
        }

        ani.Rebind();
        ani.Play(LayerType);
    }
    //-----------------------------------------------------------------------
    string GetAnimatorClip(eUIAniType type)
    {
        string clipName = "";
        //switch(type)
        //{
            //case eUIAniType.IconPoint_01:       clipName = "UI/UIAnim/Icon_Point_01";       break;
            //case eUIAniType.IconPoint_02_Num:   clipName = "UI/UIAnim/Icon_Point_02_Num";   break;
            //case eUIAniType.Messege_9Point:     clipName = "UI/UIAnim/Messege_9Point";      break;
            //case eUIAniType.Messege_BG_01:      clipName = "UI/UIAnim/Messege_BG_01";       break;
            //case eUIAniType.Messege_BG_02:      clipName = "UI/UIAnim/Messege_BG_02";       break;
            //case eUIAniType.Messege_BG_03:      clipName = "UI/UIAnim/Messege_BG_03";       break;
            //case eUIAniType.Messege_Player1Win: clipName = "UI/UIAnim/Messege_Player1Win";  break;
            //case eUIAniType.Messege_RoundStart: clipName = "UI/UIAnim/Messege_RoundStart";  break;
            //case eUIAniType.Box_Bottom:         clipName = "UI/UIAnim/Box_Bottom";          break;
            //case eUIAniType.Box_Glow:           clipName = "UI/UIAnim/Box_Glow";            break;
            //case eUIAniType.Box_Result_Text:    clipName = "UI/UIAnim/Box_Result_Text";     break;
            //case eUIAniType.Trophy:             clipName = "UI/UIAnim/Trophy_01";           break;
            //case eUIAniType.Perfect_Text_01:    clipName = "UI/UIAnim/Perfect_Text_01";     break;
            //case eUIAniType.Score_Messege:      clipName = "UI/UIAnim/Score_Messege";       break;
        //}

        return clipName;
    }
    //-----------------------------------------------------------------------
//    public Vector2 ChangeMousePos(Vector2 pos)
//    {
//        Vector2 cPos = pos;
//#if (UNITY_EDITOR || UNITY_EDITOR_64)
//#else
//        var sm = Model.First<SettingModel>();
//        if (sm._touchFull)
//        {
//            if (Display.displays[0].renderingWidth < pos.x)
//                cPos.x -= Display.displays[0].renderingWidth;
//            else if (pos.x < 0)
//                cPos.x += Display.displays[1].renderingWidth;

//            cPos.y -= Display.displays[0].renderingHeight - Display.displays[1].renderingHeight;
//            //cPos.y = Display.displays[0].renderingHeight - cPos.y;
//        }
//        else
//        {
//            cPos.x *= (float)Display.displays[1].renderingWidth / (float)Display.displays[0].renderingWidth;
//            cPos.y *= (float)Display.displays[1].renderingHeight / (float)Display.displays[0].renderingHeight;
//        }

//        //LogContent.log(string.Format("보정_X : {0}      보정_Y : {1}", cPos.x, cPos.y));
//#endif

//        return cPos;
//    }
    //-----------------------------------------------------------------------
    public IEnumerator LoadSprite(string path, Image image)
    {
        if (image == null)
            yield break;

        string localizingPath = Model.First<SettingModel>().GetLocalizingPath();
        string fullPath = string.Format("UIs/Textures/{0}/{1}", localizingPath, path);
        yield return StartCoroutine(ResourceLoader.Instance.Load<Sprite>(fullPath,
                o =>
                {
                    if (image != null)
                        image.sprite = Instantiate(o) as Sprite;
                }));
    }
    //-----------------------------------------------------------------------
}
