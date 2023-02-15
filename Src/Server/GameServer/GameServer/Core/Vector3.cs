using GameServer.Core;
using System;
using System.ComponentModel;


namespace GameServer.Core
{


    public struct Vector3 : IEquatable<Vector3>
    {
        public const float kEpsilon = 1E-05f;
        public const float kEpsilonNormalSqrt = 1E-15f;
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0.0f;
        }


        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Clamp01(t);
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }

        public static Vector3 MoveTowards(
            Vector3 current,
            Vector3 target,
            float maxDistanceDelta)
        {
            var vector3 = target - current;
            var magnitude = vector3.magnitude;
            return magnitude <= (double)maxDistanceDelta || magnitude < 1.401298464324817E-45
                ? target
                : current + vector3 / magnitude * maxDistanceDelta;
        }


        public static Vector3 SmoothDamp(
            Vector3 current,
            Vector3 target,
            ref Vector3 currentVelocity,
            float smoothTime,
            float maxSpeed)
        {
            var deltaTime = Time.deltaTime;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }


        public static Vector3 SmoothDamp(
            Vector3 current,
            Vector3 target,
            ref Vector3 currentVelocity,
            float smoothTime)
        {
            var deltaTime = Time.deltaTime;
            var maxSpeed = float.PositiveInfinity;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static Vector3 SmoothDamp(
            Vector3 current,
            Vector3 target,
            ref Vector3 currentVelocity,
            float smoothTime,
            [DefaultValue("(float)Math.Infinity")] float maxSpeed,
            [DefaultValue("Time.DeltaTime")] float deltaTime)
        {
            smoothTime = Math.Max(0.0001f, smoothTime);
            var num1 = 2f / smoothTime;
            var num2 = num1 * deltaTime;
            var num3 = (float)(1.0 / (1.0 + num2 + 0.47999998927116394 * num2 * num2 +
                                      0.23499999940395355 * num2 * num2 * num2));
            var vector = current - target;
            var vector3_1 = target;
            var maxLength = maxSpeed * smoothTime;
            var vector3_2 = ClampMagnitude(vector, maxLength);
            target = current - vector3_2;
            var vector3_3 = (currentVelocity + num1 * vector3_2) * deltaTime;
            currentVelocity = (currentVelocity - num1 * vector3_3) * num3;
            var vector3_4 = target + (vector3_2 + vector3_3) * num3;
            if (Dot(vector3_1 - current, vector3_4 - vector3_1) > 0.0)
            {
                vector3_4 = vector3_1;
                currentVelocity = (vector3_4 - vector3_1) / deltaTime;
            }

            return vector3_4;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        public void Set(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public void Scale(Vector3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                (float)(lhs.y * (double)rhs.z - lhs.z * (double)rhs.y),
                (float)(lhs.z * (double)rhs.x - lhs.x * (double)rhs.z),
                (float)(lhs.x * (double)rhs.y - lhs.y * (double)rhs.x));
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }

        public override bool Equals(object other)
        {
            return other is Vector3 other1 && Equals(other1);
        }

        public bool Equals(Vector3 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            return -2f * Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        public static Vector3 Normalize(Vector3 value)
        {
            var num = Magnitude(value);
            return num > 9.999999747378752E-06 ? value / num : zero;
        }

        public void Normalize()
        {
            var num = Magnitude(this);
            if (num > 9.999999747378752E-06)
                this = this / num;
            else
                this = zero;
        }

        public Vector3 normalized => Normalize(this);

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return (float)(lhs.x * (double)rhs.x +
                           lhs.y * (double)rhs.y +
                           lhs.z * (double)rhs.z);
        }

        public static float Angle(Vector3 from, Vector3 to)
        {
            var num = (float)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            return num < 1.0000000036274937E-15
                ? 0.0f
                : (float)Math.Acos(Clamp(Dot(from, to) / num, -1f, 1f)) * 57.29578f;
        }

        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            return Angle(from, to) * Math.Sign(Dot(axis, Cross(from, to)));
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            var vector3 = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return (float)Math.Sqrt((float)(vector3.x * (double)vector3.x + vector3.y * (double)vector3.y +
                                            vector3.z * (double)vector3.z));
        }

        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            return vector.sqrMagnitude > maxLength * (double)maxLength
                ? vector.normalized * maxLength
                : vector;
        }

        public static float Magnitude(Vector3 vector)
        {
            return (float)Math.Sqrt((float)(vector.x * (double)vector.x +
                                            vector.y * (double)vector.y +
                                            vector.z * (double)vector.z));
        }

        public float magnitude => (float)Math.Sqrt((float)(x * (double)x + y * (double)y +
                                                           z * (double)z));

        public static float SqrMagnitude(Vector3 vector)
        {
            return (float)(vector.x * (double)vector.x +
                           vector.y * (double)vector.y +
                           vector.z * (double)vector.z);
        }

        public float sqrMagnitude => (float)(x * (double)x + y * (double)y +
                                             z * (double)z);

        public static Vector3 Min(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Min(lhs.x, rhs.x),
                Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }

        public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Max(lhs.x, rhs.x),
                Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }

        public static Vector3 zero { get; } = new Vector3(0.0f, 0.0f, 0.0f);

        public static Vector3 one { get; } = new Vector3(1f, 1f, 1f);

        public static Vector3 forward { get; } = new Vector3(0.0f, 0.0f, 1f);

        public static Vector3 back { get; } = new Vector3(0.0f, 0.0f, -1f);

        public static Vector3 up { get; } = new Vector3(0.0f, 1f, 0.0f);

        public static Vector3 down { get; } = new Vector3(0.0f, -1f, 0.0f);

        public static Vector3 left { get; } = new Vector3(-1f, 0.0f, 0.0f);

        public static Vector3 right { get; } = new Vector3(1f, 0.0f, 0.0f);

        public static Vector3 positiveInfinity { get; } =
            new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        public static Vector3 negativeInfinity { get; } =
            new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        public static Vector3 operator *(Vector3 a, float d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator *(float d, Vector3 a)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return SqrMagnitude(lhs - rhs) < 9.999999439624929E-11;
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1})", x, y, z);
        }

        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2})", x.ToString(format),
                y.ToString(format), z.ToString(format));
        }

        [Obsolete("Use Vector3.forward instead.")]
        public static Vector3 fwd => new Vector3(0.0f, 0.0f, 1f);

        [Obsolete(
            "Use Vector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
        public static float AngleBetween(Vector3 from, Vector3 to)
        {
            return (float)Math.Acos(Clamp(Dot(from.normalized, to.normalized), -1f, 1f));
        }


        public static float Clamp(float value, float min, float max)
        {
            if (value < (double)min)
                value = min;
            else if (value > (double)max)
                value = max;
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static float Clamp01(float value)
        {
            if (value < 0.0)
                return 0.0f;
            return value > 1.0 ? 1f : value;
        }

        public static implicit operator Vector3(Vector3Int v)
        {
            return new Vector3 { x = v.x, y = v.y, z = v.z };
        }
    }
}