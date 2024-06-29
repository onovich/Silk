using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Silk {

    public class GLContext {

        Camera mainCamera;

        Dictionary<Material, Queue<Action>> event_queue_triangle_strip;
        Dictionary<Material, Queue<Action>> event_queue_triangle;
        Dictionary<Material, Queue<Action>> event_queue_line_strip;

        public GLContext() {
            event_queue_triangle_strip = new Dictionary<Material, Queue<Action>>();
            event_queue_triangle = new Dictionary<Material, Queue<Action>>();
            event_queue_line_strip = new Dictionary<Material, Queue<Action>>();
        }

        public void SetCamera(Camera camera) {
            mainCamera = camera;
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

        public void Triangle_Execute(Action<Material> begin, Action end) {
            foreach (var kv in event_queue_triangle) {
                begin.Invoke(kv.Key);
                while (kv.Value.Count > 0) {
                    kv.Value.Dequeue().Invoke();
                    GL.Vertex3(float.NaN, float.NaN, float.NaN);
                }
                end.Invoke();
            }
        }

        public void Triangle_Enqueue(Material material, Action action) {
            if (!event_queue_triangle.ContainsKey(material)) {
                event_queue_triangle[material] = new Queue<Action>();
            }
            event_queue_triangle[material].Enqueue(action);
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