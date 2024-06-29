using UnityEngine;

namespace MortiseFrame.Silk {

    public class GLCore {

        GLContext ctx;

        public GLCore() {
            ctx = new GLContext();
        }

        public void Tick() {

            ctx.TriangleStrip_Execute((materai) => {
                materai.SetPass(0);
                GL.Begin(GL.TRIANGLE_STRIP);
            }, () => {
                GL.End();
            });

            ctx.Triangle_Execute((materai) => {
                materai.SetPass(0);
                GL.Begin(GL.TRIANGLES);
            }, () => {
                GL.End();
            });

            ctx.Lines_Execute((materai) => {
                materai.SetPass(0);
                GL.Begin(GL.LINES);
            }, () => {
                GL.End();
            });

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

        public void DrawRect(Camera camera, Material borderMaterial, Material fillMaterial, Vector2 center, Vector2 size, Color color, float pixelThickness = 1.0f, bool fill = false) {
            var min = center - size / 2;
            var max = center + size / 2;

            if (fill) {
                fillMaterial.SetPass(0);
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(color);

                GL.Vertex3(min.x, min.y, 0);
                GL.Vertex3(max.x, min.y, 0);
                GL.Vertex3(max.x, max.y, 0);
                GL.Vertex3(min.x, max.y, 0);
                GL.Vertex3(min.x, min.y, 0);

                GL.End();
            }

            DrawLine(camera, borderMaterial, new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0), color, pixelThickness);
            DrawLine(camera, borderMaterial, new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0), color, pixelThickness);
            DrawLine(camera, borderMaterial, new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0), color, pixelThickness);
            DrawLine(camera, borderMaterial, new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0), color, pixelThickness);
        }

        public void DrawCircle(Camera camera, Material borderMaterial, Material fillMaterial, Vector3 center, float radius, Color color, int segments = 64, float pixelThickness = 1.0f, bool fill = false) {
            if (fill) {
                fillMaterial.SetPass(0);
                GL.Begin(GL.TRIANGLES);
                GL.Color(color);

                for (int i = 0; i < segments; i++) {
                    float angle1 = 2 * Mathf.PI * i / segments;
                    float angle2 = 2 * Mathf.PI * (i + 1) / segments;

                    Vector3 vertex1 = new Vector3(center.x + Mathf.Cos(angle1) * radius, center.y + Mathf.Sin(angle1) * radius, center.z);
                    Vector3 vertex2 = new Vector3(center.x + Mathf.Cos(angle2) * radius, center.y + Mathf.Sin(angle2) * radius, center.z);

                    GL.Vertex(center);
                    GL.Vertex(vertex1);
                    GL.Vertex(vertex2);
                }

                GL.End();
            }

            float thickness = PixelToWorld(pixelThickness, camera);

            borderMaterial.SetPass(0);
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

        public void DrawRing(Camera camera, Material borderMaterial, Material fillMaterial, Vector3 center, float outerRadius, float pixelThickness, Color color, bool fill = false, int segments = 64) {
            float innerRadius = outerRadius - PixelToWorld(pixelThickness, camera);

            if (fill) {
                fillMaterial.SetPass(0);
                GL.Begin(GL.TRIANGLES);
                GL.Color(color);

                for (int i = 0; i < segments; i++) {
                    float angle1 = 2 * Mathf.PI * i / segments;
                    float angle2 = 2 * Mathf.PI * (i + 1) / segments;

                    Vector3 vertex1_outer = new Vector3(center.x + Mathf.Cos(angle1) * outerRadius, center.y + Mathf.Sin(angle1) * outerRadius, center.z);
                    Vector3 vertex2_outer = new Vector3(center.x + Mathf.Cos(angle2) * outerRadius, center.y + Mathf.Sin(angle2) * outerRadius, center.z);

                    Vector3 vertex1_inner = new Vector3(center.x + Mathf.Cos(angle1) * innerRadius, center.y + Mathf.Sin(angle1) * innerRadius, center.z);
                    Vector3 vertex2_inner = new Vector3(center.x + Mathf.Cos(angle2) * innerRadius, center.y + Mathf.Sin(angle2) * innerRadius, center.z);

                    // Triangle 1
                    GL.Vertex(vertex1_inner);
                    GL.Vertex(vertex1_outer);
                    GL.Vertex(vertex2_outer);

                    // Triangle 2
                    GL.Vertex(vertex1_inner);
                    GL.Vertex(vertex2_outer);
                    GL.Vertex(vertex2_inner);
                }

                GL.End();
            } else {
                // Draw the outer circle without fill
                DrawCircle(camera, borderMaterial, fillMaterial, center, outerRadius, color, segments, 1, false);
                // Draw the inner circle without fill
                DrawCircle(camera, borderMaterial, fillMaterial, center, innerRadius, color, segments, 1, false);
            }
        }

        public void DrawStar(Camera camera, Material material, Vector3 center, int points, float innerRadius, float outerRadius, Color color, float pixelThickness = 1.0f, bool fill = false) {
            if (fill) {
                material.SetPass(0);
                GL.Begin(GL.TRIANGLES);
                GL.Color(color);

                for (int i = 0; i < points * 2; i++) {
                    float angle1 = Mathf.PI * i / points;
                    float angle2 = Mathf.PI * (i + 1) / points;

                    float radius1 = (i % 2 == 0) ? outerRadius : innerRadius;
                    float radius2 = ((i + 1) % 2 == 0) ? outerRadius : innerRadius;

                    Vector3 vertex1 = new Vector3(center.x + Mathf.Cos(angle1) * radius1, center.y + Mathf.Sin(angle1) * radius1, center.z);
                    Vector3 vertex2 = new Vector3(center.x + Mathf.Cos(angle2) * radius2, center.y + Mathf.Sin(angle2) * radius2, center.z);

                    GL.Vertex(center);
                    GL.Vertex(vertex1);
                    GL.Vertex(vertex2);
                }

                GL.End();
            }

            material.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(color);

            for (int i = 0; i < points * 2; i++) {
                float angle1 = Mathf.PI * i / points;
                float angle2 = Mathf.PI * (i + 1) / points;

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