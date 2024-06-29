using UnityEngine;

namespace MortiseFrame.Silk.Sample {

    public class SampleEntry : MonoBehaviour {

        void OnPostRender() {
        }

        void Start(){
            Camera.main.allowMSAA = true;
        }

        void OnRenderObject() {
            GLUtil.DrawCircle(Vector3.zero, 1, Color.red);
        }

    }

}