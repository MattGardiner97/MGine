using MGine.Components;
using MGine.Core;
using MGine.Interfaces;
using MGine.Shaders.ShaderStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGine.Services
{
    public class LightService : IService
    {
        private Engine engine;
        private Dictionary<PointLight, int> pointLightIndices = new Dictionary<PointLight, int>();
        private Dictionary<DirectionalLight, int> directionalLightIndices = new Dictionary<DirectionalLight, int>();
        private Stack<int> availablePointIndices = new Stack<int>();
        private Stack<int> availableDirectionalIndices = new Stack<int>();

        public PointLightStructure[] PointLightStructures { get; private set; } = new PointLightStructure[Constants.MaxLightCounts.POINT];
        public DirectionalLightStructure[] DirectionalLightStructures { get; private set; } = new DirectionalLightStructure[Constants.MaxLightCounts.DIRECTIONAL];
        public bool PointLightUpdated { get; private set; } = true;
        public bool DirectionalLightUpdated { get; private set; } = true;

        public LightService(Engine Engine)
        {
            this.engine = Engine;

            availablePointIndices = new Stack<int>(Enumerable.Range(0, Constants.MaxLightCounts.POINT).Reverse());
            availableDirectionalIndices = new Stack<int>(Enumerable.Range(0, Constants.MaxLightCounts.DIRECTIONAL).Reverse());
        }

        public void Dispose()
        {
        }

        public void Init()
        {
        }

        public void RegisterLight(Light Light)
        {
            switch(Light)
            {
                case PointLight pLight:
                    RegisterLight(pLight);
                    break;
                case DirectionalLight dLight:
                    RegisterLight(dLight);
                    break;
            }
        }
        public void RegisterLight(PointLight Light)
        {
            AddLightIndex(Light, pointLightIndices, availablePointIndices, Constants.MaxLightCounts.POINT, "point");
        }
        public void RegisterLight(DirectionalLight Light)
        {
            AddLightIndex(Light, directionalLightIndices, availableDirectionalIndices, Constants.MaxLightCounts.DIRECTIONAL, "directional");
        }

        public void Unregister(PointLight Light) { RemoveLightIndex(Light, pointLightIndices, availablePointIndices); }
        public void Unregister(DirectionalLight Light) { RemoveLightIndex(Light, directionalLightIndices, availableDirectionalIndices); }

        private void AddLightIndex<TLight>(TLight Light, Dictionary<TLight, int> Indices, Stack<int> AvailableIndices, int MaxLights, string LightName) where TLight : Light
        {
            if (Indices.Count >= MaxLights)
                throw new ArgumentException($"Cannot register more than {MaxLights} {LightName} lights.");

            int index = AvailableIndices.Pop();
            Indices.Add(Light, index);
            Light.RebuildSignalled += this.RebuildLight;
        }

        private void RemoveLightIndex<TLight>(TLight Light, Dictionary<TLight, int> Indices, Stack<int> AvailableIndices) where TLight : Light
        {
            if (Indices.ContainsKey(Light))
            {
                int index = Indices[Light];
                Indices.Remove(Light);
                AvailableIndices.Push(index);
            }
            else
                throw new ArgumentException($"{Light.GetType().Name} has not been registed.");
        }

        private void RebuildLight(Light Light)
        {
            switch (Light)
            {
                case PointLight pLight:
                    if (pointLightIndices.ContainsKey(pLight))
                    {
                        int index = pointLightIndices[pLight];
                        PointLightStructures[index] = pLight.LightStructure;
                        PointLightUpdated = true;
                    }
                    break;
                case DirectionalLight dLight:
                    if(directionalLightIndices.ContainsKey(dLight))
                    {
                        int index = directionalLightIndices[dLight];
                        DirectionalLightStructures[index] = dLight.LightStructure;
                        DirectionalLightUpdated = true;
                    }
                    break;
                default:
                    throw new InvalidCastException($"{Light.GetType().FullName} is not a supported Light type");
            }
        }

        public void ResetPointLight()
        {
            this.PointLightUpdated = false;
        }

        public void ResetDirectionalLight()
        {
            this.PointLightUpdated = false;
        }
    }
}
