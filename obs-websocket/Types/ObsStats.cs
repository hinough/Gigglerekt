using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace obs_websocket.Types
{
    public class ObsStats
    {
        [JsonProperty("cpu-usage")]
        public double cpu { get; set; }

        [JsonProperty("free-disk-space")]
        public double disk { get; set; }

        [JsonProperty("fps")]
        public double fps { get; set; }

        [JsonProperty("average-frame-time")]
        public double frametime { get; set; }

        [JsonProperty("memory-usage")]
        public double memory { get; set; }


        [JsonProperty("output-total-frames")]
        public int outputtotal { get; set; }

        [JsonProperty("output-skipped-frames")]
        public int outputskipped { get; set; }

        [JsonProperty("render-missed-frames")]
        public int rendermissed { get; set; }

        [JsonProperty("render-total-frames")]
        public int rendertotal { get; set; }

        public ObsStats(double cpu, double disk, double fps, double frametime, double memory,
                        int outputtotal, int outputskipped, int rendermissed, int rendertotal)
        {
            this.cpu = cpu;
            this.disk = disk;
            this.fps = fps;
            this.frametime = frametime;
            this.memory = memory;

            this.outputtotal = outputtotal;
            this.outputskipped = outputskipped;
            this.rendermissed = rendermissed;
            this.rendertotal = rendertotal;
        }
    }
}
