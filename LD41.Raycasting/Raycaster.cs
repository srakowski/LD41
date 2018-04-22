using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD41.Raycasting
{
    static class Raycaster
    {
        public static Color[] Raycast(
            int[,] mapGrid,
            Point screenDim,
            Vector2 position,
            Vector2 direction,
            Vector2 plane,
            Texture2D[] textures,
            Vector2 spritePos)
        {
            var buffer = new Color[screenDim.Y, screenDim.X];
            var textureData = new Color[textures.Length][];
            int i = 0;
            foreach (var texture in textures)
            {
                textureData[i] = new Color[texture.Width * texture.Height];
                texture.GetData(textureData[i]);
                i++;
            }

            double[] zbuffer = new double[screenDim.X];

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

                    if (mapGrid[mapPos.X, mapPos.Y] > 0)
                        break;
                }

                var perpWallDist = 0.0;
                if (!yWallHit) perpWallDist = (mapPos.X - position.X + (1 - step.X) / 2) / rayDirection.X;
                else perpWallDist = (mapPos.Y - position.Y + (1 - step.Y) / 2) / rayDirection.Y;

                zbuffer[x] = perpWallDist;

                var lineHeight = (int)(screenDim.Y / perpWallDist);

                var halfLineHeight = lineHeight / 2;
                var midScreen = screenDim.Y / 2;
                var drawStart = MathHelper.Clamp(midScreen - halfLineHeight, 0, screenDim.Y - 1);
                var drawEnd = MathHelper.Clamp(midScreen + halfLineHeight, 0, screenDim.Y - 1);

                var texIdx = mapGrid[mapPos.X, mapPos.Y] - 1;
                var tex = textures[texIdx];
                var data = textureData[texIdx];

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
                    var dataIdx = MathHelper.Clamp((tex.Width * texY) + texX, 0, data.Length - 1);
                    buffer[y, x] = new Color(data[dataIdx], light);
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

                tex = textures[0];
                data = textureData[0];
                var fdata = textureData[5];

                for (int y = drawEnd + 1; y < screenDim.Y; y++)
                {
                    currentDist = screenDim.Y / (2.0 * y - screenDim.Y);
                    light = (float)(Light(currentDist));

                    double weight = (currentDist - distPlayer) / (distWall - distPlayer);

                    double currentFloorX = weight * floorXWall + (1.0 - weight) * position.X;
                    double currentFloorY = weight * floorYWall + (1.0 - weight) * position.Y;

                    var floorTexX = 0;
                    var floorTexY = 0;
                    floorTexX = (int)(currentFloorX * tex.Width) % tex.Width;
                    floorTexY = (int)(currentFloorY * tex.Height) % tex.Height;

                    
                    var col = new Color(data[tex.Width * floorTexY + floorTexX], light);
                    buffer[y, x] = col;

                    col = new Color(fdata[tex.Width * floorTexY + floorTexX], light);
                    buffer[screenDim.Y - y, x] = col;
                }

            }

            //// SPRITE
            //Vector2 relSpritePos = spritePos - position;
            //double invDet = 1.0 / (plane.X * direction.Y - direction.X * plane.Y);
            //double transformX = invDet * (direction.Y * relSpritePos.X - direction.X * relSpritePos.Y);
            //double transformY = invDet * (-plane.Y * relSpritePos.X + plane.X * relSpritePos.Y);

            //int spriteScreenX = (int)((screenDim.X / 2) * (1 + transformX / transformY));

            //int spriteHeight = Math.Abs((int)(screenDim.Y / (transformY)));

            //int drawStartY = MathHelper.Clamp(-spriteHeight / 2 + screenDim.Y / 2, 0, screenDim.Y - 1);
            //int drawEndY = MathHelper.Clamp(spriteHeight / 2 + screenDim.Y / 2, 0, screenDim.Y - 1);

            ////calculate width of the sprite
            //int spriteWidth = Math.Abs((int)(screenDim.Y / (transformY)));
            //int drawStartX = MathHelper.Clamp(-spriteWidth / 2 + spriteScreenX, 0, screenDim.X - 1);
            //int drawEndX = MathHelper.Clamp(spriteWidth / 2 + spriteScreenX, 0, screenDim.X - 1);

            //var tex2 = textures[6];
            //var tData = textureData[6];

            ////loop through every vertical stripe of the sprite on screen
            //for (int stripe = drawStartX; stripe < drawEndX; stripe++)
            //{
            //    int texX = (int)(256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * tex2.Width / spriteWidth) / 256;
            //    if (transformY > 0 && stripe > 0 && stripe < screenDim.X && transformY < zbuffer[stripe])
            //        for (int y = drawStartY; y < drawEndY; y++)
            //        {
            //            int d = (y) * 256 - screenDim.Y * 128 + spriteHeight * 128;
            //            int texY = ((d * tex2.Height) / spriteHeight) / 256;
            //            var color = tData[tex2.Width * texY + texX];
            //            if (color.A != 0)
            //                buffer[y, stripe] = color;
            //        }
            //}

            var pixels = new Color[buffer.Length];
            var p = 0;
            foreach (var color in buffer)
                pixels[p++] = color;

            return pixels;
        }

        private static double Light(double currentDist) => 1.0 / (currentDist);
    }
}
