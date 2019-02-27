using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using UnityEngine;

namespace Unity.UIWidgets.animation {
    public abstract class Curve {
        public abstract float transform(float t);

        public Curve flipped {
            get { return new FlippedCurve(this); }
        }

        public override string ToString() {
            return this.GetType().ToString();
        }
    }

    class _Linear : Curve {
        public override float transform(float t) {
            return t;
        }
    }

    public class SawTooth : Curve {
        public SawTooth(int count) {
            this.count = count;
        }

        public readonly int count;

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0);
            if (t == 1.0f) {
                return 1.0f;
            }

            t *= this.count;
            return t - (int) t;
        }

        public override string ToString() {
            return $"{this.GetType()}({this.count})";
        }
    }

    public class Interval : Curve {
        public Interval(float begin, float end, Curve curve = null) {
            this.begin = begin;
            this.end = end;
            this.curve = curve ?? Curves.linear;
        }

        public readonly float begin;

        public readonly float end;

        public readonly Curve curve;

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0);
            D.assert(this.begin >= 0.0);
            D.assert(this.begin <= 1.0);
            D.assert(this.end >= 0.0);
            D.assert(this.end <= 1.0);
            D.assert(this.end >= this.begin);
            if (t == 0.0 || t == 1.0) {
                return t;
            }

            t = ((t - this.begin) / (this.end - this.begin)).clamp(0.0f, 1.0f);
            if (t == 0.0 || t == 1.0) {
                return t;
            }

            return this.curve.transform(t);
        }

        public override string ToString() {
            if (!(this.curve is _Linear)) {
                return $"{this.GetType()}({this.begin}\u22EF{this.end}\u27A9{this.curve}";
            }

            return $"{this.GetType()}({this.begin}\u22EF{this.end})";
        }
    }

    public class Threshold : Curve {
        public Threshold(float threshold) {
            this.threshold = threshold;
        }

        public readonly float threshold;

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0);
            D.assert(this.threshold >= 0.0);
            D.assert(this.threshold <= 1.0);
            if (t == 0.0 || t == 1.0) {
                return t;
            }

            return t < this.threshold ? 0.0f : 1.0f;
        }
    }

    public class Cubic : Curve {
        public Cubic(float a, float b, float c, float d) {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public readonly float a;

        public readonly float b;

        public readonly float c;

        public readonly float d;

        const float _cubicErrorBound = 0.001f;

        float _evaluateCubic(float a, float b, float m) {
            return 3 * a * (1 - m) * (1 - m) * m +
                   3 * b * (1 - m) * m * m +
                   m * m * m;
        }

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0);

            float start = 0.0f;
            float end = 1.0f;
            while (true) {
                float midpoint = (start + end) / 2;
                float estimate = this._evaluateCubic(this.a, this.c, midpoint);
                if ((t - estimate).abs() < _cubicErrorBound) {
                    return this._evaluateCubic(this.b, this.d, midpoint);
                }

                if (estimate < t) {
                    start = midpoint;
                }
                else {
                    end = midpoint;
                }
            }
        }

        public override string ToString() {
            return $"{this.GetType()}({this.a:F2}, {this.b:F2}, {this.c:F2}, {this.d:F2})";
        }
    }

    public class FlippedCurve : Curve {
        public FlippedCurve(Curve curve) {
            D.assert(curve != null);
            this.curve = curve;
        }

        public readonly Curve curve;

        public override float transform(float t) {
            return 1.0f - this.curve.transform(1.0f - t);
        }

        public override string ToString() {
            return $"{this.GetType()}({this.curve})";
        }
    }

    class _DecelerateCurve : Curve {
        internal _DecelerateCurve() {
        }

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0);
            t = 1.0f - t;
            return 1.0f - t * t;
        }
    }

    class _BounceInCurve : Curve {
        internal _BounceInCurve() {
        }

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0);
            return 1.0f - Curves._bounce(1.0f - t);
        }
    }

    class _BounceOutCurve : Curve {
        internal _BounceOutCurve() {
        }

        public override float transform(float t) {
            D.assert(t >= 0.0f && t <= 1.0f);
            return Curves._bounce(t);
        }
    }

    class _BounceInOutCurve : Curve {
        internal _BounceInOutCurve() {
        }

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0f);
            if (t < 0.5f) {
                return (1.0f - Curves._bounce(1.0f - t)) * 0.5f;
            }
            else {
                return Curves._bounce(t * 2.0f - 1.0f) * 0.5f + 0.5f;
            }
        }
    }

    public class ElasticInCurve : Curve {
        public ElasticInCurve(float period = 0.4f) {
            this.period = period;
        }

        public readonly float period;

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0);
            float s = this.period / 4.0f;
            t = t - 1.0f;
            return -Mathf.Pow(2.0f, 10.0f * t) * Mathf.Sin((t - s) * (Mathf.PI * 2.0f) / this.period);
        }

        public override string ToString() {
            return $"{this.GetType()}({this.period})";
        }
    }

    public class ElasticOutCurve : Curve {
        public ElasticOutCurve(float period = 0.4f) {
            this.period = period;
        }

        public readonly float period;

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0);
            float s = this.period / 4.0f;
            return Mathf.Pow(2.0f, -10.0f * t) * Mathf.Sin((t - s) * (Mathf.PI * 2.0f) / this.period) + 1.0f;
        }

        public override string ToString() {
            return $"{this.GetType()}({this.period})";
        }
    }

    public class ElasticInOutCurve : Curve {
        public ElasticInOutCurve(float period = 0.4f) {
            this.period = period;
        }

        public readonly float period;

        public override float transform(float t) {
            D.assert(t >= 0.0 && t <= 1.0);
            float s = this.period / 4.0f;
            t = 2.0f * t - 1.0f;
            if (t < 0.0) {
                return -0.5f * Mathf.Pow(2.0f, 10.0f * t) * Mathf.Sin((t - s) * (Mathf.PI * 2.0f) / this.period);
            }
            else {
                return Mathf.Pow(2.0f, -10.0f * t) * Mathf.Sin((t - s) * (Mathf.PI * 2.0f) / this.period) * 0.5f +
                       1.0f;
            }
        }

        public override string ToString() {
            return $"{this.GetType()}({this.period})";
        }
    }

    public static class Curves {
        public static readonly Curve linear = new _Linear();

        public static readonly Curve decelerate = new _DecelerateCurve();

        public static readonly Curve ease = new Cubic(0.25f, 0.1f, 0.25f, 1.0f);

        public static readonly Curve easeIn = new Cubic(0.42f, 0.0f, 1.0f, 1.0f);

        public static readonly Curve easeOut = new Cubic(0.0f, 0.0f, 0.58f, 1.0f);

        public static readonly Curve easeInOut = new Cubic(0.42f, 0.0f, 0.58f, 1.0f);

        public static readonly Curve fastOutSlowIn = new Cubic(0.4f, 0.0f, 0.2f, 1.0f);

        public static readonly Curve bounceIn = new _BounceInCurve();

        public static readonly Curve bounceOut = new _BounceOutCurve();

        public static readonly Curve bounceInOut = new _BounceInOutCurve();

        public static readonly Curve elasticIn = new ElasticInCurve();

        public static readonly Curve elasticOut = new ElasticOutCurve();

        public static readonly Curve elasticInOut = new ElasticInOutCurve();

        internal static float _bounce(float t) {
            if (t < 1.0f / 2.75f) {
                return 7.5625f * t * t;
            }
            else if (t < 2 / 2.75f) {
                t -= 1.5f / 2.75f;
                return 7.5625f * t * t + 0.75f;
            }
            else if (t < 2.5f / 2.75f) {
                t -= 2.25f / 2.75f;
                return 7.5625f * t * t + 0.9375f;
            }

            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }
    }
}