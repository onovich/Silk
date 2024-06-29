using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Silk {

    public class GLContext {

        bool isOrthographic;
        float orthographicSize;
        int pixelHeight;
        float fieldOfView;
        internal float pixelToWorldFactor;
        internal Vector2 cameraForward;

        Dictionary<Material, Queue<Action>> event_queue_triangle_strip;
        Dictionary<Material, Queue<Action>> event_queue_triangle;
        Dictionary<Material, Queue<Action>> event_queue_line_strip;

        public GLContext() {
            event_queue_triangle_strip = new Dictionary<Material, Queue<Action>>();
            event_queue_triangle = new Dictionary<Material, Queue<Action>>();
            event_queue_line_strip = new Dictionary<Material, Queue<Action>>();
        }

        public void RecordCameraInfo(Camera camera) {
            isOrthographic = camera.orthographic;
            orthographicSize = camera.orthographicSize;
            pixelHeight = camera.pixelHeight;
            fieldOfView = camera.fieldOfView;
            if (isOrthographic) {
                pixelToWorldFactor = orthographicSize * 2 / pixelHeight;
            } else {
                pixelToWorldFactor = Mathf.Tan(fieldOfView * Mathf.Deg2Rad * 0.5f) * 2.0f / pixelHeight;
            }
            cameraForward = camera.transform.forward;
        }

        public void TriangleStrip_Execute(Action<Material> begin, Action end) {
            foreach (var kv in event_queue_triangle_strip) {
                begin.Invoke(kv.Key);
                while (kv.Value.Count > 0) {
                    kv.Value.Dequeue().Invoke();
                    GL.Vertex3(float.NaN, float.NaN, float.NaN);
                }
                end.Invoke();
            }
        }

        public void TriangleStrip_Enqueue(Material material, Action action) {
            if (!event_queue_triangle_strip.ContainsKey(material)) {
                event_queue_triangle_strip[material] = new Queue<Action>();
            }
            event_queue_triangle_strip[material].Enqueue(action);
        }

        public void LineStrip_Execute(Action<Material> begin, Action end) {
            foreach (var kv in event_queue_line_strip) {
                begin.Invoke(kv.Key);
                while (kv.Value.Count > 0) {
                    kv.Value.Dequeue().Invoke();
                    GL.Vertex3(float.NaN, float.NaN, float.NaN);
                }
                end.Invoke();
            }
        }

        public void LineStrip_Enqueue(Material material, Action action) {
            if (!event_queue_line_strip.ContainsKey(material)) {
                event_queue_line_strip[material] = new Queue<Action>();
            }
            event_queue_line_strip[material].Enqueue(action);
        }

        public void Clear() {
            event_queue_triangle_strip.Clear();
            event_queue_triangle.Clear();
            event_queue_line_strip.Clear();
        }

    }

}