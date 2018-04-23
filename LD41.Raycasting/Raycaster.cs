using LD41.Gameplay;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace LD41.Raycasting
{
    class RaycasterTarget
    {
        public RaycasterTarget(Cell cell, double distance)
        {
            Cell = cell;
            Distance = distance;
        }
        public Cell Cell { get; }
        public double Distance { get; }
    }

    static class Raycaster
    {
        public static RaycasterTarget GetTarget(Map map,
            Point screenDim,
            Player player)
        {
            Vector2 position = player.Position;
            Vector2 direction = player.Look;
            Vector2 plane = player.Fov;

            var x = screenDim.X / 2;
            var cameraX = (2 * (x / (float)screenDim.X)) - 1; // where on the plane -1 to 1
            var rayDirection = direction + (plane * cameraX);
            var mapPos = position.ToPoint();

            var sideDist = Vector2.Zero;

            var deltaDist = Vector2.One;
            deltaDist.X /= Math.Abs(rayDirection.X);
            deltaDist.Y /= Math.Abs(rayDirection.Y);

            var step = Point.Zero;

            step.X = rayDirection.X < 0 ? -1 : 1;
            sideDist.X = rayDirection.X < 0
                ? (position.X - mapPos.X) * deltaDist.X
                : (mapPos.X + 1f - position.X) * deltaDist.X;

            step.Y = rayDirection.Y < 0 ? -1 : 1;
            sideDist.Y = rayDirection.Y < 0
                ? (position.Y - mapPos.Y) * deltaDist.Y
                : (mapPos.Y + 1f - position.Y) * deltaDist.Y;

            var yWallHit = false;
            while (true)
            {
                if (sideDist.X < sideDist.Y)
                {
                    sideDist.X += deltaDist.X;
                    mapPos.X += step.X;
                    yWallHit = false;
                }
                else
                {
                    sideDist.Y += deltaDist.Y;
                    mapPos.Y += step.Y;
                    yWallHit = true;
                }

                if (!(map[mapPos.X, mapPos.Y] is Cell.VoidCell))
                    break;
            }

            var perpWallDist = 0.0;
            if (!yWallHit) perpWallDist = (mapPos.X - position.X + (1 - step.X) / 2) / rayDirection.X;
            else perpWallDist = (mapPos.Y - position.Y + (1 - step.Y) / 2) / rayDirection.Y;

            return new RaycasterTarget(map[mapPos.X, mapPos.Y], perpWallDist);
        }

        public static Color[] Raycast(
            Map map,
            Point screenDim,
            Player player,
            Dictionary<string, TextureData> textures)
        {
            Vector2 position = player.Position;
            Vector2 direction = player.Look;
            Vector2 plane = player.Fov;

            var buffer = new Color[screenDim.Y, screenDim.X];
            //double[] zbuffer = new double[screenDim.X];
            for (int x = 0; x < screenDim.X; x++)
            {
                var cameraX = (2 * (x / (float)screenDim.X)) - 1; // where on the plane -1 to 1

                var rayDirection = direction + (plane * cameraX);

                var mapPos = position.ToPoint();

                var sideDist = Vector2.Zero;

                var deltaDist = Vector2.One;
                deltaDist.X /= Math.Abs(rayDirection.X);
                deltaDist.Y /= Math.Abs(rayDirection.Y);

                var step = Point.Zero;

                step.X = rayDirection.X < 0 ? -1 : 1;
                sideDist.X = rayDirection.X < 0
                    ? (position.X - mapPos.X) * deltaDist.X
                    : (mapPos.X + 1f - position.X) * deltaDist.X;

                step.Y = rayDirection.Y < 0 ? -1 : 1;
                sideDist.Y = rayDirection.Y < 0
                    ? (position.Y - mapPos.Y) * deltaDist.Y
                    : (mapPos.Y + 1f - position.Y) * deltaDist.Y;

                var yWallHit = false;
                while (true)
                {
                    if (sideDist.X < sideDist.Y)
                    {
                        sideDist.X += deltaDist.X;
                        mapPos.X += step.X;
                        yWallHit = false;
                    }
                    else
                    {
                        sideDist.Y += deltaDist.Y;
                        mapPos.Y += step.Y;
                        yWallHit = true;
                    }

                    if (!(map[mapPos.X, mapPos.Y] is Cell.VoidCell))
                        break;
                }

                var perpWallDist = 0.0;
                if (!yWallHit) perpWallDist = (mapPos.X - position.X + (1 - step.X) / 2) / rayDirection.X;
                else perpWallDist = (mapPos.Y - position.Y + (1 - step.Y) / 2) / rayDirection.Y;

                //zbuffer[x] = perpWallDist;

                var lineHeight = (int)(screenDim.Y / perpWallDist);

                var halfLineHeight = lineHeight / 2;
                var midScreen = screenDim.Y / 2;
                var drawStart = MathHelper.Clamp(midScreen - halfLineHeight, 0, screenDim.Y - 1);
                var drawEnd = MathHelper.Clamp(midScreen + halfLineHeight, 0, screenDim.Y - 1);

                var textureName = map[mapPos.X, mapPos.Y].TextureName;
                var tex = textures[textureName];

                double wallX = 0;
                if (!yWallHit) wallX = position.Y + (perpWallDist * rayDirection.Y);
                else wallX = position.X + (perpWallDist * rayDirection.X);
                wallX -= Math.Floor(wallX);

                int texX = (int)(wallX * tex.Width);
                if ((!yWallHit && rayDirection.X > 0) || (yWallHit && rayDirection.Y < 0))
                    texX = tex.Width - texX - 1;

                var light = (float)(Light(perpWallDist));

                for (int y = drawStart; y < drawEnd + 1; y++)
                {
                    int d = (y * 256) - (screenDim.Y * 128) + (lineHeight * 128);
                    int texY = ((d * tex.Height) / lineHeight) / 256;
                    var dataIdx = MathHelper.Clamp((tex.Width * texY) + texX, 0, tex.Data.Length - 1);
                    buffer[y, x] = new Color(tex.Data[dataIdx], light);
                }

                var floorXWall = 0.0;
                var floorYWall = 0.0;

                if (!yWallHit && rayDirection.X > 0)
                {
                    floorXWall = mapPos.X;
                    floorYWall = mapPos.Y + wallX;
                }
                else if (!yWallHit && rayDirection.X < 0)
                {
                    floorXWall = mapPos.X + 1.0;
                    floorYWall = mapPos.Y + wallX;
                }
                else if (yWallHit && rayDirection.Y > 0)
                {
                    floorXWall = mapPos.X + wallX;
                    floorYWall = mapPos.Y;
                }
                else
                {
                    floorXWall = mapPos.X + wallX;
                    floorYWall = mapPos.Y + 1.0;
                }

                var distWall = 0.0;
                var distPlayer = 0.0;
                var currentDist = 0.0;

                distWall = perpWallDist;
                distPlayer = 0.0;

                if (drawEnd < 0) drawEnd = screenDim.Y;

                var point = position.ToPoint();
                var floorTex = textures[(map[point.X, point.Y] as Cell.VoidCell).TextureName];
                var ceilTex = textures[(map[point.X, point.Y] as Cell.VoidCell).CeilingTextureName];

                for (int y = drawEnd + 1; y < screenDim.Y; y++)
                {
                    currentDist = screenDim.Y / (2.0 * y - screenDim.Y);
                    light = (float)(Light(currentDist));

                    double weight = (currentDist - distPlayer) / (distWall - distPlayer);

                    double currentFloorX = weight * floorXWall + (1.0 - weight) * position.X;
                    double currentFloorY = weight * floorYWall + (1.0 - weight) * position.Y;

                    var floorTexX = 0;
                    var floorTexY = 0;
                    floorTexX = (int)(currentFloorX * floorTex.Width) % floorTex.Width;
                    floorTexY = (int)(currentFloorY * floorTex.Height) % floorTex.Height;

                    var floorColIdx = floorTex.Width * floorTexY + floorTexX;
                    var col = new Color(floorTex.Data[floorColIdx], light);
                    buffer[y, x] = col;
                    col = new Color(ceilTex.Data[floorColIdx], light);
                    buffer[screenDim.Y - y, x] = col;
                }

            }


            var pixels = new Color[buffer.Length];
            var p = 0;
            foreach (var color in buffer)
                pixels[p++] = color;

            return pixels;
        }

        private static double Light(double currentDist) => 1.0 / (currentDist / 2f);
    }
}
