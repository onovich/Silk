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
        Vector2 axis;
        Vector2 agent;
        [SerializeField] float speed = 1.0f;

        void Start() {
            Camera.main.allowMSAA = useMSAA;
            core = new GLCore();
            core.RecordCameraInfo(mainCamera);
        }

        void Update() {
            axis = Vector2.zero;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) axis.y += 1;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) axis.y -= 1;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) axis.x -= 1;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) axis.x += 1;
            agent += axis.normalized * Time.deltaTime * speed;
        }

        void LateUpdate() {
            core.DrawLine(material, start, end, Color.cyan, thickness);
            if (fill) {
                core.DrawCircle(material, agent, 1, Color.yellow);
                core.DrawRing(material, new Vector3(-2, 2, 0), 1, 4, Color.red);
                core.DrawRect(material, new Vector3(2, 2, 0), Vector2.one, Color.green);
                core.DrawStar(material, new Vector3(-2, -2, 0), starPoints, starInnerRadius, starOuterRadius, Color.white);
            } else {
                core.DrawWiredCircle(material, agent, 1, Color.yellow, thickness);
                core.DrawWiredRing(material, new Vector3(-2, 2, 0), 1, thickness, Color.red);
                core.DrawWiredRect(material, new Vector3(2, 2, 0), Vector2.one, Color.green, thickness);
                core.DrawWiredStar(material, new Vector3(-2, -2, 0), starPoints, starInnerRadius, starOuterRadius, Color.white, thickness);
            }

        }

        void OnRenderObject() {
            core.Tick();
        }

        void OnDestroy() {
            core.TearDown();
        }

    }

}