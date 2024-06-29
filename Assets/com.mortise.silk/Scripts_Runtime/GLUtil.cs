using UnityEngine;

namespace MortiseFrame.Silk {

    public static class GLUtil {

        static Material lineMaterial;
        static void CreateLineMaterial() {
            if (!lineMaterial) {
                Shader shader = Shader.Find("Custom/FXAA");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        static float PixelToWorld(float pixelThickness, Camera cam) {
            if (cam.orthographic) {
                return pixelThickness * (cam.orthographicSize * 2) / cam.pixelHeight;
            } else {
                return pixelThickness * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * 2.0f / cam.pixelHeight;
            }
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float pixelThickness = 1.0f) {
            Camera cam = Camera.main;
            float thickness = PixelToWorld(pixelThickness, cam);

            CreateLineMaterial();
            lineMaterial.SetPass(0);
            GL.Begin(GL.TRIANGLE_STRIP); // 使用GL.TRIANGLE_STRIP绘制厚线
            GL.Color(color);

            Vector3 perpendicular = Vector3.Cross(end - start, cam.transform.forward).normalized * thickness / 2;

            GL.Vertex(start - perpendicular);
            GL.Vertex(start + perpendicular);
            GL.Vertex(end - perpendicular);
            GL.Vertex(end + perpendicular);

            GL.End();
        }

        public static void DrawRect(Rect rect, Color color, float pixelThickness = 1.0f) {
            Camera cam = Camera.main;
            float thickness = PixelToWorld(pixelThickness, cam);

            DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0), color, thickness);
            DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0), color, thickness);
            DrawLine(new Vector3(rect.xMax, rect.yMax, 0), new Vector3(rect.xMin, rect.yMax, 0), color, thickness);
            DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMin, rect.yMin, 0), color, thickness);
        }

        public static void DrawCircle(Vector3 center, float radius, Color color, int segments = 64, float pixelThickness = 1.0f) {
            Camera cam = Camera.main;
            float thickness = PixelToWorld(pixelThickness, cam);

            CreateLineMaterial();
            lineMaterial.SetPass(0);
            GL.Begin(GL.TRIANGLE_STRIP); // 使用GL.TRIANGLE_STRIP绘制厚圆环
            GL.Color(color);

            for (int i = 0; i <= segments; i++) {
                float angle = 2 * Mathf.PI * i / segments;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector3 inner = new Vector3(center.x + cos * (radius - thickness / 2), center.y + sin * (radius - thickness / 2), center.z);
                Vector3 outer = new Vector3(center.x + cos * (radius + thickness / 2), center.y + sin * (radius + thickness / 2), center.z);

                GL.Vertex(inner);
                GL.Vertex(outer);
            }

            GL.End();
        }
    }
}