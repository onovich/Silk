using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Silk {

    public class GLContext {

        Camera mainCamera;

        Dictionary<Material, Queue<Action>> event_queue_triangle_strip;
        Dictionary<Material, Queue<Action>> event_queue_triangle;
        Dictionary<Material, Queue<Action>> event_queue_lines;

        public GLContext() {
            event_queue_triangle_strip = new Dictionary<Material, Queue<Action>>();
            event_queue_triangle = new Dictionary<Material, Queue<Action>>();
            event_queue_lines = new Dictionary<Material, Queue<Action>>();
        }

        public void SetCamera(Camera camera) {
            mainCamera = camera;
        }

        public void TriangleStrip_Execute(Action<Material> begin, Action end) {
            foreach (var kv in event_queue_triangle_strip) {
                while (kv.Value.Count > 0) {
                    begin.Invoke(kv.Key);
                    kv.Value.Dequeue().Invoke();
                    end.Invoke();
                }
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
                while (kv.Value.Count > 0) {
                    begin.Invoke(kv.Key);
                    kv.Value.Dequeue().Invoke();
                    end.Invoke();
                }
            }
        }

        public void Triangle_Enqueue(Material material, Action action) {
            if (!event_queue_triangle.ContainsKey(material)) {
                event_queue_triangle[material] = new Queue<Action>();
            }
            event_queue_triangle[material].Enqueue(action);
        }

        public void Lines_Execute(Action<Material> begin, Action end) {
            foreach (var kv in event_queue_lines) {
                while (kv.Value.Count > 0) {
                    begin.Invoke(kv.Key);
                    kv.Value.Dequeue().Invoke();
                    end.Invoke();
                }
            }
        }

        public void Lines_Enqueue(Material material, Action action) {
            if (!event_queue_lines.ContainsKey(material)) {
                event_queue_lines[material] = new Queue<Action>();
            }
            event_queue_lines[material].Enqueue(action);
        }

        public void Clear() {
            event_queue_triangle_strip.Clear();
        }

    }

}