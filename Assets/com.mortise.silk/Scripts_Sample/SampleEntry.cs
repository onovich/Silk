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
        [SerializeField] bool fill;

        GLCore core;

        void Start() {
            Camera.main.allowMSAA = useMSAA;
            core = new GLCore();
        }

        void OnRenderObject() {
            core.DrawRing(mainCamera, material, material, new Vector3(-2, 2, 0), 1, 4, Color.red, fill: fill);
            core.DrawLine(mainCamera, material, start, end, Color.green, thickness);
            core.DrawRect(mainCamera, material, material, Vector2.zero, new Vector2(2, 2), Color.red, thickness, fill: fill);
            core.DrawCircle(mainCamera, material, material, new Vector3(2, 2, 0), 1, Color.blue, fill: fill);
            core.DrawStar(mainCamera, material, new Vector3(-2, -2, 0), starPoints, starInnerRadius, starOuterRadius, Color.white, thickness, fill: fill);
            core.Tick();
        }

    }

}