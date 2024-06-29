using UnityEngine;

namespace MortiseFrame.Silk {

    public class GLCore {

        GLContext ctx;

        public GLCore() {
            ctx = new GLContext();
        }

        public Material CreateMaterial(Shader shader) {
            // Shader shader = Shader.Find("Custom/FXAA");
            var mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            return mat;
        }

        float PixelToWorld(float pixelThickness, Camera cam) {
            if (cam.orthographic) {
                return pixelThickness * (cam.orthographicSize * 2) / cam.pixelHeight;
            } else {
                return pixelThickness * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * 2.0f / cam.pixelHeight;
            }
        }

        public void DrawLine(Camera camera, Material material, Vector3 start, Vector3 end, Color color, float pixelThickness = 1.0f) {
            float thickness = PixelToWorld(pixelThickness, camera);

            material.SetPass(0);
            GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(color);

            Vector3 perpendicular = Vector3.Cross(end - start, camera.transform.forward).normalized * thickness / 2;

            GL.Vertex(start - perpendicular);
            GL.Vertex(start + perpendicular);
            GL.Vertex(end - perpendicular);
            GL.Vertex(end + perpendicular);

            GL.End();
        }

        public void DrawRect(Camera camera, Material material, Vector2 center, Vector2 size, Color color, float pixelThickness = 1.0f) {
            var min = center - size / 2;
            var max = center + size / 2;
            DrawLine(camera, material, new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0), color, pixelThickness);
            DrawLine(camera, material, new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0), color, pixelThickness);
            DrawLine(camera, material, new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0), color, pixelThickness);
            DrawLine(camera, material, new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0), color, pixelThickness);
        }

        public void DrawCircle(Camera camera, Material material, Vector3 center, float radius, Color color, int segments = 64, float pixelThickness = 1.0f) {
            float thickness = PixelToWorld(pixelThickness, camera);

            material.SetPass(0);
            GL.Begin(GL.TRIANGLE_STRIP);
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

        public void DrawStar(Camera camera, Material material, Vector3 center, int points, float innerRadius, float outerRadius, Color color, float pixelThickness = 1.0f) {
            material.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(color);

            for (int i = 0; i < points * 2; i++) {
                float angle1 = 2 * Mathf.PI * i / (points * 2);
                float angle2 = 2 * Mathf.PI * (i + 1) / (points * 2);

                float radius1 = (i % 2 == 0) ? outerRadius : innerRadius;
                float radius2 = ((i + 1) % 2 == 0) ? outerRadius : innerRadius;

                Vector3 point1 = new Vector3(center.x + Mathf.Cos(angle1) * radius1, center.y + Mathf.Sin(angle1) * radius1, center.z);
                Vector3 point2 = new Vector3(center.x + Mathf.Cos(angle2) * radius2, center.y + Mathf.Sin(angle2) * radius2, center.z);

                GL.Vertex(point1);
                GL.Vertex(point2);
            }

            GL.End();
        }
    }
}