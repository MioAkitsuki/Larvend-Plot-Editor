using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Larvend.PlotEditor
{
    public struct NextCommandEvent {}
    public class StageController : MonoSingleton<StageController>
    {
        private Button panel;

        private Button backButton;

        private Button finish;

        void Awake()
        {
            panel = transform.Find("Panel").GetComponent<Button>();
            panel.onClick.AddListener(() =>
            {
                TypeEventSystem.Global.Send<NextCommandEvent>();
            });

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() =>
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            });

            finish = transform.Find("Finish").GetComponent<Button>();
            finish.onClick.AddListener(() =>
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            });
        }
    }
}