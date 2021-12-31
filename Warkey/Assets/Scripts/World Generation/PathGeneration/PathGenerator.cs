using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
public static class PathGenerator
{
    /*
     * @param direction (1,1) bot right, (-1,1) bot left, (1,-1) top right, (-1,-1) top left
     */
    /*
    public static PathData GeneratePath(int seed, float[,] heightMap, Vector2 startPos, Vector2 direction, PathSettings pathSettings, bool isBranch = false) {
        if (direction.x == 0 && direction.y == 0) direction = new Vector2(1, 1);
        System.Random random = new Random(seed);
        PathData pathData = new PathData();
        pathData.start = startPos;
        pathData.points.Add(startPos);

        int xLength = heightMap.GetLength(0);
        int yLength = heightMap.GetLength(1);


        float[,] pathMap = new float[xLength, yLength];

        float minValue = pathSettings.heightDifference.min;
        float maxValue = pathSettings.heightDifference.max;
        float maxSteep = pathSettings.maxSteepChange;

        int maxIteration = (!isBranch) ? pathSettings.maxIteration : pathSettings.branchIteration;
        int maxSubIteration = pathSettings.maxSubIteration;
        int iteration = 0;

        Vector2 canditate = new Vector2(startPos.x, startPos.y);
        Vector2 nextCandidate;
        pathMap[(int)canditate.x, (int)canditate.y] = 1;
        while (iteration < maxIteration) {
            iteration++;
            int subIteration = 0;

            float negativeStrength;

            bool isOnHeightLimit = false;
            bool isOnSteepLimit = false;
            bool isInBounds = false;
            float xModifier, yModifier;
            do {
                subIteration++;
                negativeStrength = Mathf.Min(1, ((!isBranch) ? pathSettings.negativeStrength : pathSettings.branchNegativeStrength * (1 + iteration / maxIteration)) + subIteration / (float)maxSubIteration);
                int dist = random.Next((int)pathSettings.distance.min, (int)pathSettings.distance.max);

                if (random.NextDouble() > negativeStrength)
                    xModifier = (float)RandomHelper.Range(0, 1f, ref random);
                else
                    xModifier = (float)RandomHelper.Range(-1f, 0, ref random);

                if (random.NextDouble() > negativeStrength)
                    yModifier = (float)RandomHelper.Range(0, 1f, ref random);
                else
                    yModifier = (float)RandomHelper.Range(-1f, 0, ref random);

                nextCandidate = new Vector2(xModifier * dist * direction.x + canditate.x, yModifier * dist * direction.y + canditate.y);

                isInBounds = IsInBounds(nextCandidate);
                if (!isInBounds) break;
                isOnHeightLimit = isInBounds && (heightMap[(int)nextCandidate.x, (int)nextCandidate.y] <= maxValue && heightMap[(int)nextCandidate.x, (int)nextCandidate.y] >= minValue);
                isOnSteepLimit = isInBounds && (Mathf.Abs(heightMap[(int)nextCandidate.x, (int)nextCandidate.y] - heightMap[(int)canditate.x, (int)canditate.y]) <= maxSteep);

            }
            while (subIteration < maxSubIteration && (!isInBounds || ((!isOnHeightLimit || !isOnSteepLimit))));


            if (isInBounds && isOnHeightLimit && isOnSteepLimit) {
                float width = ((!isBranch) ? pathSettings.pathWidth : pathSettings.branchWidth);
                canditate.x = nextCandidate.x; canditate.y = nextCandidate.y;
                pathMap[(int)canditate.x, (int)canditate.y] = 1;
                pathData.points.Add(canditate);

                for (float i = 0; i <= width; i += 0.50f) {
                    for (int angle = 0; angle < 360; angle += 5) {
                        Vector2 vect = MathHelper.Project(canditate, width, angle);
                        pathMap[(int)Mathf.Floor(Mathf.Clamp(vect.x, 0, pathMap.GetLength(0) - 1)), (int)Mathf.Floor(Mathf.Clamp(vect.y, 0, pathMap.GetLength(1) - 1))] = 1;
                    }
                }


                if (IsOver(canditate)) break;

                if (!isBranch && random.NextDouble() <= pathSettings.branchChance) {
                    Vector2 branchDirection = new Vector2(direction.x, direction.y) * ((random.Next(0, 2) == 0) ? new Vector2(1, -1) : new Vector2(-1, 1));
                    PathData branch = GeneratePath(seed, heightMap, canditate, branchDirection, pathSettings, true);
                    for (int y = 0; y < yLength; y++) {
                        for (int x = 0; x < xLength; x++) {
                            if (branch.pathMap[x, y] == 1) {
                                pathMap[x, y] = branch.pathMap[x, y];
                            }
                        }
                    }

                    pathData.branches.Add(branch);

                }
            }
            else {
                break;
            }
        }


        bool IsInBounds(Vector2 vector) {
            return (vector.x >= 0 && vector.x < xLength && vector.y >= 0 && vector.y < yLength);
        }

        bool IsOver(Vector2 vector) {
            const int dist = 5;
            bool isOver;
            isOver = (direction.x > 0) ? vector.x >= xLength - dist : vector.x <= dist;
            isOver = isOver && ((direction.y > 0) ? vector.y >= yLength - dist : vector.y <= dist);
            return isOver;
        }

        pathData.pathMap = pathMap;
        pathData.end = canditate;

        return pathData;
    }
    */
    
    public static PathData GeneratePath(PathSettings pathSettings,PathData pathData,float[,] heightMap01, Vector2 direction, int branchIndex = -1) {
        Random random = new Random(pathSettings.seed);
        XY length = new XY(heightMap01.GetLength(0), heightMap01.GetLength(1));

        int maxMainIteration = (branchIndex == -1) ? pathSettings.maxIteration : pathSettings.branchSettings[branchIndex].branchIteration;
        int maxSubIteration =   (branchIndex == -1)? pathSettings.maxSubIteration : pathSettings.maxSubIteration;
        int iteration = 0;

        if(pathData.start == null) {
            pathData.SetStart(pathData.start);
        }
        pathData.direction = direction;

        Vector2 previousCandidate = new Vector2(pathData.start.x,pathData.start.y);
        Vector2 candidate = new Vector2();

        while(iteration < maxMainIteration) {
            iteration++;

            int subIteration = 0;
            Candidate topCandidate = new Candidate(Vector2.zero,-20,-20);
            while (subIteration < maxSubIteration) {
                subIteration++;

                float distance = RandomHelper.Range(pathSettings.distance.min, pathSettings.distance.max, ref random);
                float negativeStrength = ((branchIndex == -1) ? pathSettings.negativeStrength : pathSettings.branchSettings[branchIndex].branchNegativeStrength) + subIteration/ maxSubIteration;
                float xScale, yScale;

                if (random.NextDouble() > negativeStrength)
                    xScale = (float)RandomHelper.Range(0, 1f, ref random);
                else
                    xScale = (float)RandomHelper.Range(-1f, 0, ref random);
                if (random.NextDouble() > negativeStrength)
                    yScale = (float)RandomHelper.Range(0, 1f, ref random);
                else
                    yScale = (float)RandomHelper.Range(-1f, 0, ref random);

                candidate.x = previousCandidate.x + xScale * distance * direction.x;
                candidate.y = previousCandidate.y + yScale * distance * direction.y;

                if (!IsInBounds(candidate)) continue;

                float height = heightMap01[(int)candidate.x, (int)candidate.y];
                float steepDifference = Mathf.Abs(height - heightMap01[(int)previousCandidate.x, (int)previousCandidate.y]);

                float sScore = (pathSettings.maxSteepChange - steepDifference)*5;
                float hScore = Mathf.Min((height - pathSettings.height.min),(pathSettings.height.max - height));

                if (sScore+hScore > topCandidate.steepScore + topCandidate.heightScore) {
                    topCandidate.point = candidate;
                    topCandidate.steepScore = sScore;
                    topCandidate.heightScore = hScore;

                }
                if (sScore <= 0 || hScore <= 0)
                    continue;
                else
                    break;
            }

            if (true) {
                float radius = ((branchIndex == -1) ? pathSettings.pathWidth : pathSettings.branchSettings[branchIndex].branchWidth);
                previousCandidate.x = candidate.x;
                previousCandidate.y = candidate.y;
                pathData.pathMap[(int)candidate.x, (int)candidate.y] = 1;
                pathData.points.Add(candidate);

                for (float i = 0; i <= radius; i += 0.50f) {
                    for (int angle = 0; angle < 360; angle += 5) {
                        Vector2 vect = MathHelper.Project(candidate, i, angle);
                        pathData.pathMap[(int)Mathf.Floor(Mathf.Clamp(vect.x, 0, length.x - 1)), (int)Mathf.Floor(Mathf.Clamp(vect.y, 0, length.y - 1))] = 1;

                        if (i < radius / 2 && IsOnCorner(vect)) {
                            pathData.endingCorner = GetCorner(vect);
                            pathData.hasFinished = true;
                        }
                    }
                }

                if (pathData.hasFinished) {
                    pathData.end = candidate;
                    break;
                }
                else if (pathSettings.branchSettings.Length > branchIndex+1 && random.NextDouble() <= pathSettings.branchSettings[branchIndex+1].branchChance) {
                    Vector2 branchDirection = new Vector2(direction.x, direction.y) * ((random.Next(0, 2) == 0) ? new Vector2(1, -1) : new Vector2(-1, 1));
                    PathData branch = new PathData(length.x, length.y);
                    branch.isBranch = true;
                    branch.SetStart(candidate);
                    branch = GeneratePath(pathSettings, branch, heightMap01, branchDirection, branchIndex+1);
                    for (int y = 0; y < length.y; y++) {
                        for (int x = 0; x < length.x; x++) {
                            if (branch.pathMap[x, y] == 1) {
                                pathData.pathMap[x, y] = branch.pathMap[x, y];
                            }
                        }
                    }
                    pathData.branches.Add(branch);
                }

            }
        }


        bool IsInBounds(Vector2 vector) {
            return (vector.x >= 0 && vector.x < length.x && vector.y >= 0 && vector.y < length.y);
        }

        bool IsOnCorner(Vector2 vector) {
            const int dist = 20;
            return (pathData.start-vector).magnitude > 25 && (vector.x >= length.x - dist || vector.x <= dist || vector.y >= length.y - dist || vector.y <= dist);
        }

        //direction (1,1) bot right, (-1,1) bot left, (1,-1) top right, (-1,-1) top left
        //Bot Left (0,240)
        Vector2 GetCorner(Vector2 vector) {
            const int distance = 20;
            Vector2 corner = Vector2.negativeInfinity;
            if ((vector.x <= distance || vector.x > length.x - distance))
                corner.x = (Mathf.Abs(length.x - vector.x) > vector.x) ? 1 : 0;
            if ((vector.y <= distance || vector.y > length.y - distance))
                corner.y = (Mathf.Abs(length.y - vector.y) > vector.y) ? 0 : 1;

            return corner;
        }

        return pathData;
    }



    public static float[,] SetPathHeight(float[,] path, float[,] heightmap, PathSettings pathSettings) {
        int xLength = path.GetLength(0);
        int yLength = path.GetLength(1);

        float[,] heightPath = new float[xLength, yLength];
        for (int y = 0; y < yLength; y++) {
            for (int x = 0; x < xLength; x++) {
                if (path[x, y] == 1) {
                    heightPath[x, y] = heightmap[x, y] + pathSettings.pathHeightOffset;
                }
                else {
                    heightPath[x, y] = -1;
                }

            }
        }

        for (int z = 0; z < 2; z++) {
            float[,] heightSmoothedPath = (float[,])heightPath.Clone();
            for (int y = 0; y < yLength; y++) {
                for (int x = 0; x < xLength; x++) {
                    if (heightPath[x, y] == -1) {
                        float weight = 0;
                        for (int i = 1; i <= 5; i++) {
                            float scale = 1 / i;
                            if (x + i < xLength && heightPath[x + i, y] != -1) weight += 1 * scale;
                            if (y + i < yLength && heightPath[x, y + i] != -1) weight += 1 * scale;
                            if (x - i > 0 && heightPath[x - i, y] != -1) weight += 1 * scale;
                            if (y - i > 0 && heightPath[x, y - i] != -1) weight += 1 * scale;

                            scale /= 2;
                            if (x + i < xLength && y + i < yLength && heightPath[x + i, y + i] != -1) weight += 1 * scale;
                            if (x + i < xLength && y - i > 0 && heightPath[x + i, y - i] != -1) weight += 1 * scale;
                            if (x - i > 0 && y + i < yLength && heightPath[x - i, y + i] != -1) weight += 1 * scale;
                            if (x - i > 0 && y - i > 0 && heightPath[x - i, y - i] != -1) weight += 1 * scale;

                        }
                        if (weight > pathSettings.smoothWeight)
                            heightSmoothedPath[x, y] = heightmap[x, y] + pathSettings.pathHeightOffset;
                    }
                }
            }
            heightPath = heightSmoothedPath;
        }


        float[,] heightSmootPath = new float[xLength, yLength];
        for (int y = 0; y < yLength; y++) {
            for (int x = 0; x < xLength; x++) {
                if (heightPath[x, y] != -1) {
                    for (int i = 1; i <= 10; i++) {
                        float scale = ((i * .001f));
                        if (x + i < xLength && heightPath[x + i, y] == -1) heightSmootPath[x + i, y] = heightmap[x + i, y] - scale + pathSettings.pathHeightOffset;
                        if (y + i < yLength && heightPath[x, y + i] == -1) heightSmootPath[x, y + i] = heightmap[x, y + i] - scale + pathSettings.pathHeightOffset;
                        if (x - i > 0 && heightPath[x - i, y] == -1) heightSmootPath[x - i, y] = heightmap[x - i, y] - scale + pathSettings.pathHeightOffset;
                        if (y - i > 0 && heightPath[x, y - i] == -1) heightSmootPath[x, y - i] = heightmap[x, y - i] - scale + pathSettings.pathHeightOffset;
                    }
                }
            }
        }

        for (int y = 0; y < yLength; y++) {
            for (int x = 0; x < xLength; x++) {
                heightPath[x, y] += heightSmootPath[x, y];

            }
        }


        return heightPath;
    }

   
    public static PathData GenerateStartPosition(PathSettings pathSettings,PathData pathData ,float[,] heightMap01, Vector2 starCorner) {
        float distance = 0;
        XY length = new XY(heightMap01.GetLength(0), heightMap01.GetLength(1));
        Candidate candidate = new Candidate(starCorner, -1000, -1000);
        while (distance < Mathf.Max(length.x,length.y)) {
            distance += 0.5f;
            for (float angle = 0; angle < 360; angle += 5) {
                Vector2 center = MathHelper.Project(starCorner, distance, angle);
                if (!IsInBounds(center)) continue;
                bool isGood = true;
                float heightScore = 0;
                for(int i = 0; i < pathSettings.startRadius; i++) {
                    Vector2 vect = MathHelper.Project(starCorner, distance, angle);
                    if (!IsInBounds(vect)) {
                        isGood = false;
                        break;
                    }
                    else if(heightMap01[(int)vect.x,(int)vect.y] > pathSettings.height.max || heightMap01[(int)vect.x, (int)vect.y] < pathSettings.height.min) {
                        heightScore += Mathf.Min((heightMap01[(int)vect.x, (int)vect.y] - pathSettings.height.min), (pathSettings.height.max - heightMap01[(int)vect.x, (int)vect.y]));
                    }
                }
                if (isGood) {
                    float dist = (float)-Vector2.Distance(starCorner, candidate.point)/100;
                    if (candidate.heightScore+candidate.steepScore < heightScore+dist) {
                        candidate.heightScore = heightScore;
                        candidate.steepScore = dist;
                        candidate.point = center;
                    }
                }
            }
        }

        for (float i = 0; i <= pathSettings.startRadius; i += 0.50f) {
            for (float angle = 0; angle < 360; angle += 5 / (1 + pathSettings.startRadius / 5)) {
                Vector2 vect = MathHelper.Project(candidate.point, i, angle);
                pathData.pathMap[(int)Mathf.Floor(Mathf.Clamp(vect.x, 0, length.x - 1)), (int)Mathf.Floor(Mathf.Clamp(vect.y, 0, length.y - 1))] = 1;
            }
        }

        pathData.SetStart(candidate.point);

        bool IsInBounds(Vector2 vector) {
            return (vector.x >= 0 && vector.x < length.x && vector.y >= 0 && vector.y < length.y);
        }

        return pathData;
    }


    public static PathData GeneratePath(PathSettings pathSettings,HeightMap heightMap, Vector2 startCorner, Vector2 direction){
        PathData pathData = new PathData(heightMap.values01.GetLength(0), heightMap.values01.GetLength(1));
        pathData = GenerateStartPosition(pathSettings, pathData, heightMap.values01,startCorner);
        pathData = GeneratePath(pathSettings, pathData, heightMap.values01, direction);
        pathData.heightMap = SetPathHeight(pathData.pathMap, heightMap.values, pathSettings);
        return pathData;
    }

    public static PathData GeneratePath(PathSettings pathSettings, float[,] heightMap, Vector2 startCorner, Vector2 direction) {
        PathData pathData = new PathData(heightMap.GetLength(0), heightMap.GetLength(1));
        pathData = GenerateStartPosition(pathSettings, pathData, heightMap, startCorner);
        pathData = GeneratePath(pathSettings, pathData, heightMap,direction);
        return pathData;
    }

    public static PathData AddPath(PathSettings pathSettings,PathData pathData ,HeightMap heightMap) {
        PathData newPathData = new PathData(heightMap.values01.GetLength(0), heightMap.values01.GetLength(1));
        newPathData = GeneratePath(pathSettings, pathData, heightMap.values01, pathData.direction,0);
        for(int x = 0; x < pathData.pathMap.GetLength(0); x++) {
            for(int y = 0; x < pathData.pathMap.GetLength(1); y++) {
                if(pathData.pathMap[x, y] == -1)
                    pathData.pathMap[x, y] = newPathData.pathMap[x, y];
            }
        }
        pathData.heightMap = SetPathHeight(pathData.pathMap, heightMap.values, pathSettings);
        return pathData;
    }
}


struct Candidate
{
    public Vector2 point;
    public float heightScore;
    public float steepScore;

    public Candidate(Vector2 point, float heightScore, float steepScore) {
        this.point = point;
        this.heightScore = heightScore;
        this.steepScore = steepScore;
    }
}

public class PathData
{
    public float[,] pathMap;
    public float[,] heightMap;
    public Vector2 direction;
    public Vector2 start;
    public Vector2 end;
    public bool isBranch;
    public List<Vector2> points = new List<Vector2>();
    public List<PathData> branches = new List<PathData>();

    public Vector2 endingCorner = Vector2.zero;
    public bool hasFinished;

    public PathData(int sizeX, int sizeY) {
        pathMap = new float[sizeX, sizeY];
        heightMap = new float[sizeX, sizeY];
    }

    public PathData(float[,] pathMap) {
        this.pathMap = pathMap;
        heightMap = new float[pathMap.GetLength(0), pathMap.GetLength(1)];
    }

    public void SetStart(Vector2 start) {
        this.start = start;
        if (points.Count != 0)
            points.Insert(0, start);
        else
            points.Add(start);
    }

    public int sizeX { get => pathMap.GetLength(0); }
    public int sizeY { get => pathMap.GetLength(1);}
}