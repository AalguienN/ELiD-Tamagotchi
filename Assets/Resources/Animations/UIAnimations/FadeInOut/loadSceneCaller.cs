using UnityEngine;
public class loadSceneCaller : MonoBehaviour
{
    public void CallLoadScene(){
        SceneChangeManager.instance.goingToChange = true;
        SceneChangeManager.LoadSceneDirectly(0);
    }
}
