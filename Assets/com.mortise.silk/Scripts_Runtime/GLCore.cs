using System;
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

            ctx.LineStrip_Execute((materai) => {
                materai.SetPass(0);
                GL.Begin(GL.LINE_STRIP);
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

        #region Line
        public void DrawLine(Camera camera,
                             Material material,
                             Vector3 start,
                             Vector3 end,
                             Color color,
                             float pixelThickness = 1.0f) {
            if (pixelThickness <= 0) {
                return;
            }

            if (pixelThickness == 1) {
                Action task = () => {
                    GL.Color(color);
                    GL.Vertex(start);
                    GL.Vertex(end);
                };
                ctx.LineStrip_Enqueue(material, task);
            }

            if (pixelThickness > 1) {
                Action task = () => {
                    float thickness = PixelToWorld(pixelThickness, camera);
                    GL.Color(color);
                    Vector3 perpendicular = Vector3.Cross(end - start, camera.transform.forward).normalized * thickness / 2;

                    GL.Vertex(start - perpendicular);
                    GL.Vertex(start + perpendicular);
                    GL.Vertex(end - perpendicular);
                    GL.Vertex(end + perpendicular);
                };
                ctx.TriangleStrip_Enqueue(material, task);
            }
        }
        #endregion

        #region Rect
        public void DrawRect(Camera camera,
                             Material material,
                             Vector2 center,
                             Vector2 size,
                             Color color) {
            var min = center - size / 2;
            var max = center + size / 2;

            Action task = () => {
                GL.Color(color);

                GL.Vertex3(min.x, min.y, 0);
                GL.Vertex3(max.x, min.y, 0);
                GL.Vertex3(max.x, max.y, 0);
                GL.Vertex3(min.x, max.y, 0);
                GL.Vertex3(min.x, min.y, 0);
            };
            ctx.TriangleStrip_Enqueue(material, task);
        }
        #endregion

        #region WiredRect
        public void DrawWiredRect(Camera camera,
                             Material material,
                             Vector2 center,
                             Vector2 size,
                             Color color,
                             float pixelThickness) {
            var min = center - size / 2;
            var max = center + size / 2;

            if (pixelThickness <= 0) {
                return;
            }

            DrawLine(camera, material, new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0), color, pixelThickness);
            DrawLine(camera, material, new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0), color, pixelThickness);
            DrawLine(camera, material, new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0), color, pixelThickness);
            DrawLine(camera, material, new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0), color, pixelThickness);
        }
        #endregion

        #region Circle
        public void DrawCircle(Camera camera,
                               Material material,
                               Vector3 center,
                               float radius,
                               Color color,
                               int segments = 64) {
            Action fillTask = () => {
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
            };
            ctx.TriangleStrip_Enqueue(material, fillTask);
        }
        #endregion

        #region WiredCircle
        public void DrawWiredCircle(Camera camera,
                              Material material,
                              Vector3 center,
                              float radius,
                              Color color,
                              float pixelThickness = 1f,
                              int segments = 64) {

            Action boundTask = () => {

                float thickness = PixelToWorld(pixelThickness, camera);
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

            };
            ctx.TriangleStrip_Enqueue(material, boundTask);
        }
        #endregion

        #region Ring
        public void DrawRing(Camera camera,
                             Material material,
                             Vector3 center,
                             float outerRadius,
                             float pixelThickness,
                             Color color,
                             int segments = 64) {
            float innerRadius = outerRadius - PixelToWorld(pixelThickness, camera);

            GL.Color(color);

            for (int i = 0; i < segments; i++) {
                float angle1 = 2 * Mathf.PI * i / segments;
                float angle2 = 2 * Mathf.PI * (i + 1) / segments;

                Vector3 vertex1_outer = new Vector3(center.x + Mathf.Cos(angle1) * outerRadius, center.y + Mathf.Sin(angle1) * outerRadius, center.z);
                Vector3 vertex2_outer = new Vector3(center.x + Mathf.Cos(angle2) * outerRadius, center.y + Mathf.Sin(angle2) * outerRadius, center.z);

                Vector3 vertex1_inner = new Vector3(center.x + Mathf.Cos(angle1) * innerRadius, center.y + Mathf.Sin(angle1) * innerRadius, center.z);
                Vector3 vertex2_inner = new Vector3(center.x + Mathf.Cos(angle2) * innerRadius, center.y + Mathf.Sin(angle2) * innerRadius, center.z);

                DrawTriangle(camera, material, vertex1_inner, vertex1_outer, vertex2_outer, color);
                DrawTriangle(camera, material, vertex1_inner, vertex2_outer, vertex2_inner, color);
            }

        }
        #endregion

        #region WiredRing
        public void DrawWiredRing(Camera camera,
                             Material material,
                             Vector3 center,
                             float outerRadius,
                             float pixelThickness,
                             Color color,
                             int segments = 64) {
            float innerRadius = outerRadius - PixelToWorld(pixelThickness, camera);

            DrawWiredCircle(camera, material, center, outerRadius, color, pixelThickness, segments);
            DrawWiredCircle(camera, material, center, innerRadius, color, pixelThickness, segments);
        }
        #endregion

        #region Triangle
        public void DrawTriangle(Camera camera,
                                 Material material,
                                 Vector3 a,
                                 Vector3 b,
                                 Vector3 c,
                                 Color color) {
            Action task = () => {
                GL.Color(color);
                GL.Vertex(a);
                GL.Vertex(b);
                GL.Vertex(c);
            };
            ctx.TriangleStrip_Enqueue(material, task);
        }
        #endregion

        #region WiredTriangle
        public void DrawWiredTriangle(Camera camera,
                                 Material material,
                                 Vector3 a,
                                 Vector3 b,
                                 Vector3 c,
                                 Color color,
                                 float pixelThickness) {
            DrawLine(camera, material, a, b, color, pixelThickness);
            DrawLine(camera, material, b, c, color, pixelThickness);
            DrawLine(camera, material, c, a, color, pixelThickness);
        }
        #endregion

        #region Star
        public void DrawStar(Camera camera,
                             Material material,
                             Vector3 center,
                             int points,
                             float innerRadius,
                             float outerRadius,
                             Color color) {

            for (int i = 0; i < points * 2; i++) {
                float angle1 = Mathf.PI * i / points;
                float angle2 = Mathf.PI * (i + 1) / points;

                float radius1 = (i % 2 == 0) ? outerRadius : innerRadius;
                float radius2 = ((i + 1) % 2 == 0) ? outerRadius : innerRadius;

                Vector3 vertex1 = new Vector3(center.x + Mathf.Cos(angle1) * radius1, center.y + Mathf.Sin(angle1) * radius1, center.z);
                Vector3 vertex2 = new Vector3(center.x + Mathf.Cos(angle2) * radius2, center.y + Mathf.Sin(angle2) * radius2, center.z);
                DrawTriangle(camera, material, center, vertex1, vertex2, color);
            }
        }
        #endregion

        #region WiredStar
        public void DrawWiredStar(Camera camera,
                             Material material,
                             Vector3 center,
                             int points,
                             float innerRadius,
                             float outerRadius,
                             Color color,
                             float pixelThickness) {

            GL.Color(color);
            for (int i = 0; i < points * 2; i++) {
                float angle1 = Mathf.PI * i / points;
                float angle2 = Mathf.PI * (i + 1) / points;

                float radius1 = (i % 2 == 0) ? outerRadius : innerRadius;
                float radius2 = ((i + 1) % 2 == 0) ? outerRadius : innerRadius;

                Vector3 point1 = new Vector3(center.x + Mathf.Cos(angle1) * radius1, center.y + Mathf.Sin(angle1) * radius1, center.z);
                Vector3 point2 = new Vector3(center.x + Mathf.Cos(angle2) * radius2, center.y + Mathf.Sin(angle2) * radius2, center.z);

                DrawLine(camera, material, point1, point2, color, pixelThickness);
            }
        }
        #endregion

        public void TearDown() {
            ctx.Clear();
        }

    }
}