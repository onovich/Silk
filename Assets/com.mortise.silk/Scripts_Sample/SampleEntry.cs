using UnityEngine;

namespace MortiseFrame.Silk.Sample {

    public class SampleEntry : MonoBehaviour {

        [SerializeField] Vector2 start;
        [SerializeField] Vector2 end;
        [SerializeField] float thickness = 1.0f;
        [SerializeField] Shader shader;
        [SerializeField] Material material;
        [SerializeField] Camera mainCamera;
        [SerializeField] bool useMSAA;

        [SerializeField] float starInnerRadius;
        [SerializeField] float starOuterRadius;
        [SerializeField] int starPoints = 3;

        GLCore core;

        void Start() {
            Camera.main.allowMSAA = useMSAA;
            core = new GLCore();
        }

        void OnRenderObject() {
            core.DrawCircle(mainCamera, material, Vector3.zero, 1, Color.red);
            core.DrawLine(mainCamera, material, start, end, Color.green, thickness);
            core.DrawRect(mainCamera, material, Vector2.zero, new Vector2(2, 2), Color.red, thickness);
            core.DrawStar(mainCamera, material, Vector2.zero, starPoints, starInnerRadius, starOuterRadius, Color.white, thickness);
        }

    }

}