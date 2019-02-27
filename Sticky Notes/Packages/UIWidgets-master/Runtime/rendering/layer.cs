﻿using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;

namespace Unity.UIWidgets.rendering {
    public abstract class Layer : AbstractNodeMixinDiagnosticableTree {
        public new ContainerLayer parent {
            get { return (ContainerLayer) base.parent; }
        }

        bool _needsAddToScene = true;

        protected void markNeedsAddToScene() {
            this._needsAddToScene = true;
        }

        protected virtual bool alwaysNeedsAddToScene {
            get { return false; }
        }

        internal bool _subtreeNeedsAddToScene;

        flow.Layer _engineLayer;

        internal virtual void updateSubtreeNeedsAddToScene() {
            this._subtreeNeedsAddToScene = this._needsAddToScene || this.alwaysNeedsAddToScene;
        }

        public Layer nextSibling {
            get { return this._nextSibling; }
        }

        internal Layer _nextSibling;

        public Layer previousSibling {
            get { return this._previousSibling; }
        }

        internal Layer _previousSibling;

        protected override void dropChild(AbstractNodeMixinDiagnosticableTree child) {
            this.markNeedsAddToScene();
            base.dropChild(child);
        }

        protected override void adoptChild(AbstractNodeMixinDiagnosticableTree child) {
            this.markNeedsAddToScene();
            base.adoptChild(child);
        }

        public virtual void remove() {
            if (this.parent != null) {
                this.parent._removeChild(this);
            }
        }

        public void replaceWith(Layer newLayer) {
            D.assert(this.parent != null);
            D.assert(this.attached == this.parent.attached);
            D.assert(newLayer.parent == null);
            D.assert(newLayer._nextSibling == null);
            D.assert(newLayer._previousSibling == null);
            D.assert(!newLayer.attached);

            newLayer._nextSibling = this.nextSibling;
            if (this._nextSibling != null) {
                this._nextSibling._previousSibling = newLayer;
            }

            newLayer._previousSibling = this.previousSibling;
            if (this._previousSibling != null) {
                this._previousSibling._nextSibling = newLayer;
            }

            D.assert(() => {
                Layer node = this;
                while (node.parent != null) {
                    node = node.parent;
                }

                D.assert(node != newLayer);
                return true;
            });

            this.parent.adoptChild(newLayer);
            D.assert(newLayer.attached == this.parent.attached);

            if (this.parent.firstChild == this) {
                this.parent._firstChild = newLayer;
            }

            if (this.parent.lastChild == this) {
                this.parent._lastChild = newLayer;
            }

            this._nextSibling = null;
            this._previousSibling = null;
            this.parent.dropChild(this);
            D.assert(!this.attached);
        }

        internal abstract flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null);

        internal void _addToSceneWithRetainedRendering(SceneBuilder builder) {
            if (!this._subtreeNeedsAddToScene && this._engineLayer != null) {
                builder.addRetained(this._engineLayer);
                return;
            }

            this._engineLayer = this.addToScene(builder);
            this._needsAddToScene = false;
        }

        public object debugCreator;

        public override string toStringShort() {
            return base.toStringShort() + (this.owner == null ? " DETACHED" : "");
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new DiagnosticsProperty<object>("owner", this.owner,
                level: this.parent != null ? DiagnosticLevel.hidden : DiagnosticLevel.info,
                defaultValue: Diagnostics.kNullDefaultValue));
            properties.add(new DiagnosticsProperty<object>("creator", this.debugCreator,
                defaultValue: Diagnostics.kNullDefaultValue, level: DiagnosticLevel.debug));
        }
    }

    public class PictureLayer : Layer {
        public PictureLayer(Rect canvasBounds) {
            this.canvasBounds = canvasBounds;
        }

        public readonly Rect canvasBounds;

        Picture _picture;

        public Picture picture {
            get { return this._picture; }
            set {
                this.markNeedsAddToScene();
                this._picture = value;
            }
        }

        bool _isComplexHint = false;

        public bool isComplexHint {
            get { return this._isComplexHint; }
            set {
                if (value != this._isComplexHint) {
                    this._isComplexHint = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        bool _willChangeHint = false;

        public bool willChangeHint {
            get { return this._willChangeHint; }
            set {
                if (value != this._willChangeHint) {
                    this._willChangeHint = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            layerOffset = layerOffset ?? Offset.zero;

            builder.addPicture(layerOffset, this.picture,
                isComplexHint: this.isComplexHint, willChangeHint: this.willChangeHint);
            return null;
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new DiagnosticsProperty<Rect>("paint bounds", this.canvasBounds));
        }
    }

    public class ContainerLayer : Layer {
        public Layer firstChild {
            get { return this._firstChild; }
        }

        internal Layer _firstChild;

        public Layer lastChild {
            get { return this._lastChild; }
        }

        internal Layer _lastChild;

        bool _debugUltimatePreviousSiblingOf(Layer child, Layer equals = null) {
            D.assert(child.attached == this.attached);
            while (child.previousSibling != null) {
                D.assert(child.previousSibling != child);
                child = child.previousSibling;
                D.assert(child.attached == this.attached);
            }

            return child == equals;
        }

        bool _debugUltimateNextSiblingOf(Layer child, Layer equals = null) {
            D.assert(child.attached == this.attached);
            while (child._nextSibling != null) {
                D.assert(child._nextSibling != child);
                child = child._nextSibling;
                D.assert(child.attached == this.attached);
            }

            return child == equals;
        }

        internal override void updateSubtreeNeedsAddToScene() {
            base.updateSubtreeNeedsAddToScene();
            Layer child = this.firstChild;
            while (child != null) {
                child.updateSubtreeNeedsAddToScene();
                this._subtreeNeedsAddToScene = this._subtreeNeedsAddToScene || child._subtreeNeedsAddToScene;
                child = child.nextSibling;
            }
        }

        public override void attach(object owner) {
            base.attach(owner);

            var child = this.firstChild;
            while (child != null) {
                child.attach(owner);
                child = child.nextSibling;
            }
        }

        public override void detach() {
            base.detach();

            var child = this.firstChild;
            while (child != null) {
                child.detach();
                child = child.nextSibling;
            }
        }

        public void append(Layer child) {
            D.assert(child != this);
            D.assert(child != this.firstChild);
            D.assert(child != this.lastChild);
            D.assert(child.parent == null);
            D.assert(!child.attached);
            D.assert(child.nextSibling == null);
            D.assert(child.previousSibling == null);
            D.assert(() => {
                Layer node = this;
                while (node.parent != null) {
                    node = node.parent;
                }

                D.assert(node != child);
                return true;
            });

            this.adoptChild(child);
            child._previousSibling = this.lastChild;
            if (this.lastChild != null) {
                this.lastChild._nextSibling = child;
            }

            this._lastChild = child;
            if (this._firstChild == null) {
                this._firstChild = child;
            }

            D.assert(child.attached == this.attached);
        }

        internal void _removeChild(Layer child) {
            D.assert(child.parent == this);
            D.assert(child.attached == this.attached);
            D.assert(this._debugUltimatePreviousSiblingOf(child, equals: this.firstChild));
            D.assert(this._debugUltimateNextSiblingOf(child, equals: this.lastChild));

            if (child._previousSibling == null) {
                D.assert(this.firstChild == child);
                this._firstChild = child.nextSibling;
            } else {
                child._previousSibling._nextSibling = child.nextSibling;
            }

            if (child._nextSibling == null) {
                D.assert(this.lastChild == child);
                this._lastChild = child.previousSibling;
            } else {
                child._nextSibling._previousSibling = child.previousSibling;
            }

            D.assert((this.firstChild == null) == (this.lastChild == null));
            D.assert(this.firstChild == null || this.firstChild.attached == this.attached);
            D.assert(this.lastChild == null || this.lastChild.attached == this.attached);
            D.assert(this.firstChild == null ||
                     this._debugUltimateNextSiblingOf(this.firstChild, equals: this.lastChild));
            D.assert(this.lastChild == null ||
                     this._debugUltimatePreviousSiblingOf(this.lastChild, equals: this.firstChild));

            child._nextSibling = null;
            child._previousSibling = null;
            this.dropChild(child);
            D.assert(!child.attached);
        }

        public void removeAllChildren() {
            Layer child = this.firstChild;
            while (child != null) {
                Layer next = child.nextSibling;
                child._previousSibling = null;
                child._nextSibling = null;
                D.assert(child.attached == this.attached);
                this.dropChild(child);
                child = next;
            }

            this._firstChild = null;
            this._lastChild = null;
        }

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            this.addChildrenToScene(builder, layerOffset);
            return null;
        }

        public void addChildrenToScene(SceneBuilder builder, Offset childOffset = null) {
            Layer child = this.firstChild;
            while (child != null) {
                if (childOffset == null || childOffset == Offset.zero) {
                    child._addToSceneWithRetainedRendering(builder);
                } else {
                    child.addToScene(builder, childOffset);
                }
                child = child.nextSibling;
            }
        }

        public virtual void applyTransform(Layer child, Matrix3 transform) {
            D.assert(child != null);
            D.assert(transform != null);
        }

        public override List<DiagnosticsNode> debugDescribeChildren() {
            var children = new List<DiagnosticsNode>();
            if (this.firstChild == null) {
                return children;
            }

            Layer child = this.firstChild;
            int count = 1;
            while (true) {
                children.Add(child.toDiagnosticsNode(name: "child " + count));
                if (child == this.lastChild) {
                    break;
                }

                count += 1;
                child = child.nextSibling;
            }

            return children;
        }
    }

    public class OffsetLayer : ContainerLayer {
        public OffsetLayer(Offset offset = null) {
            this._offset = offset ?? Offset.zero;
        }

        Offset _offset;

        public Offset offset {
            get { return this._offset; }
            set {
                value = value ?? Offset.zero;
                if (value != this._offset) {
                    this._offset = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        public override void applyTransform(Layer child, Matrix3 transform) {
            D.assert(child != null);
            D.assert(transform != null);
            transform.preTranslate((float) this.offset.dx, (float) this.offset.dy);
        }

        public Scene buildScene(SceneBuilder builder) {
            this.updateSubtreeNeedsAddToScene();
            this.addToScene(builder);
            return builder.build();
        }

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            layerOffset = layerOffset ?? Offset.zero;

            var engineLayer = builder.pushOffset(
                (float) (layerOffset.dx + this.offset.dx),
                (float) (layerOffset.dy + this.offset.dy));
            this.addChildrenToScene(builder);
            builder.pop();
            return engineLayer;
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new DiagnosticsProperty<Offset>("offset", this.offset));
        }
    }

    public class ClipRectLayer : ContainerLayer {
        public ClipRectLayer(
            Rect clipRect = null,
            Clip clipBehavior = Clip.hardEdge
        ) {
            D.assert(clipRect != null);
            D.assert(clipBehavior != Clip.none);
            this._clipRect = clipRect;
            this._clipBehavior = clipBehavior;
        }

        Rect _clipRect;

        public Rect clipRect {
            get { return this._clipRect; }
            set {
                if (value != this._clipRect) {
                    this._clipRect = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        Clip _clipBehavior;

        public Clip clipBehavior {
            get { return this._clipBehavior; }
            set {
                D.assert(value != Clip.none);
                if (value != this._clipBehavior) {
                    this._clipBehavior = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            layerOffset = layerOffset ?? Offset.zero;
            
            bool enabled = true;
            D.assert(() => {
                enabled = !D.debugDisableClipLayers;
                return true;
            });

            if (enabled) {
                builder.pushClipRect(this.clipRect.shift(layerOffset));
            }

            this.addChildrenToScene(builder, layerOffset);

            if (enabled) {
                builder.pop();
            }

            return null;
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new DiagnosticsProperty<Rect>("clipRect", this.clipRect));
        }
    }

    public class ClipRRectLayer : ContainerLayer {
        public ClipRRectLayer(
            RRect clipRRect = null,
            Clip clipBehavior = Clip.hardEdge
        ) {
            D.assert(clipRRect != null);
            D.assert(clipBehavior != Clip.none);
            this._clipRRect = clipRRect;
            this._clipBehavior = clipBehavior;
        }

        RRect _clipRRect;

        public RRect clipRRect {
            get { return this._clipRRect; }
            set {
                if (value != this._clipRRect) {
                    this._clipRRect = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        Clip _clipBehavior;

        public Clip clipBehavior {
            get { return this._clipBehavior; }
            set {
                D.assert(value != Clip.none);
                if (value != this._clipBehavior) {
                    this._clipBehavior = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            bool enabled = true;
            D.assert(() => {
                enabled = !D.debugDisableClipLayers;
                return true;
            });

            if (enabled) {
                builder.pushClipRRect(this.clipRRect.shift(layerOffset));
            }

            this.addChildrenToScene(builder, layerOffset);

            if (enabled) {
                builder.pop();
            }

            return null;
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new DiagnosticsProperty<RRect>("clipRRect", this.clipRRect));
        }
    }

    public class ClipPathLayer : ContainerLayer {
        public ClipPathLayer(
            Path clipPath = null,
            Clip clipBehavior = Clip.hardEdge
        ) {
            D.assert(clipPath != null);
            D.assert(clipBehavior != Clip.none);
            this._clipPath = clipPath;
            this._clipBehavior = clipBehavior;
        }

        Path _clipPath;

        public Path clipPath {
            get { return this._clipPath; }
            set {
                if (value != this._clipPath) {
                    this._clipPath = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        Clip _clipBehavior;

        public Clip clipBehavior {
            get { return this._clipBehavior; }
            set {
                D.assert(value != Clip.none);
                if (value != this._clipBehavior) {
                    this._clipBehavior = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            bool enabled = true;
            D.assert(() => {
                enabled = !D.debugDisableClipLayers;
                return true;
            });

            if (enabled) {
                builder.pushClipPath(this.clipPath.shift(layerOffset));
            }

            this.addChildrenToScene(builder, layerOffset);

            if (enabled) {
                builder.pop();
            }

            return null;
        }
    }

    public class TransformLayer : OffsetLayer {
        public TransformLayer(Matrix3 transform = null, Offset offset = null) : base(offset) {
            this._transform = transform ?? Matrix3.I();
        }

        public Matrix3 transform {
            get { return this._transform; }
            set { this._transform = value; }
        }

        Matrix3 _transform;
        Matrix3 _lastEffectiveTransform;

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            layerOffset = layerOffset ?? Offset.zero;
            
            this._lastEffectiveTransform = this._transform;

            var totalOffset = this.offset + layerOffset;
            if (totalOffset != Offset.zero) {
                this._lastEffectiveTransform = Matrix3.makeTrans(totalOffset.dx, totalOffset.dy);
                this._lastEffectiveTransform.preConcat(this._transform);
            }

            builder.pushTransform(this._lastEffectiveTransform);
            this.addChildrenToScene(builder);
            builder.pop();
            return null;
        }

        public override void applyTransform(Layer child, Matrix3 transform) {
            D.assert(child != null);
            D.assert(transform != null);
            transform.preConcat(this._lastEffectiveTransform);
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new DiagnosticsProperty<Matrix3>("transform", this.transform));
        }
    }

    public class OpacityLayer : ContainerLayer {
        public OpacityLayer(int alpha = 255, Offset offset = null) {
            this._alpha = alpha;
            this._offset = offset ?? Offset.zero;
        }

        int _alpha;

        public int alpha {
            get { return this._alpha; }
            set {
                if (value != this._alpha) {
                    this._alpha = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        Offset _offset;

        public Offset offset {
            get { return this._offset; }
            set {
                value = value ?? Offset.zero;
                if (value != this._offset) {
                    this._offset = value;
                    this.markNeedsAddToScene();
                }
            }
        }

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            layerOffset = layerOffset ?? Offset.zero;

            bool enabled = true;
            D.assert(() => {
                enabled = !D.debugDisableOpacityLayers;
                return true;
            });
            if (enabled) {
                builder.pushOpacity(this.alpha, offset: this.offset + layerOffset);
            }

            this.addChildrenToScene(builder, layerOffset);
            if (enabled) {
                builder.pop();
            }

            return null;
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new IntProperty("alpha", this.alpha));
            properties.add(new DiagnosticsProperty<Offset>("offset", this.offset));
        }
    }

    public class LayerLink {
        public LeaderLayer leader {
            get { return this._leader; }
        }

        internal LeaderLayer _leader;

        public override string ToString() {
            return $"{Diagnostics.describeIdentity(this)}({(this._leader != null ? "<linked>" : "<dangling>")})";
        }
    }

    public class LeaderLayer : ContainerLayer {
        public LeaderLayer(LayerLink link, Offset offset = null) {
            D.assert(link != null);
            offset = offset ?? Offset.zero;
            this.link = link;
            this.offset = offset;
        }

        public readonly LayerLink link;

        public Offset offset;

        protected override bool alwaysNeedsAddToScene {
            get { return true; }
        }

        public override void attach(object owner) {
            base.attach(owner);
            D.assert(this.link.leader == null);
            this._lastOffset = null;
            this.link._leader = this;
        }

        public override void detach() {
            D.assert(this.link.leader == this);
            this.link._leader = null;
            this._lastOffset = null;
            base.detach();
        }

        internal Offset _lastOffset;

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            layerOffset = layerOffset ?? Offset.zero;

            D.assert(this.offset != null);
            this._lastOffset = this.offset + layerOffset;
            if (this._lastOffset != Offset.zero) {
                builder.pushTransform(Matrix3.makeTrans(this._lastOffset));
            }

            this.addChildrenToScene(builder, Offset.zero);
            if (this._lastOffset != Offset.zero) {
                builder.pop();
            }

            return null;
        }

        public override void applyTransform(Layer child, Matrix3 transform) {
            D.assert(this._lastOffset != null);
            if (this._lastOffset != Offset.zero) {
                transform.preTranslate(this._lastOffset.dx, this._lastOffset.dy);
            }
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new DiagnosticsProperty<Offset>("offset", this.offset));
            properties.add(new DiagnosticsProperty<LayerLink>("link", this.link));
        }
    }

    public class FollowerLayer : ContainerLayer {
        public FollowerLayer(
            LayerLink link = null,
            bool showWhenUnlinked = true,
            Offset unlinkedOffset = null,
            Offset linkedOffset = null
        ) {
            D.assert(link != null);
            this.link = link;
            this.showWhenUnlinked = showWhenUnlinked;
            this.unlinkedOffset = unlinkedOffset ?? Offset.zero;
            this.linkedOffset = linkedOffset ?? Offset.zero;
        }

        public readonly LayerLink link;
        public bool showWhenUnlinked;
        public Offset unlinkedOffset;
        public Offset linkedOffset;

        Offset _lastOffset;
        Matrix3 _lastTransform;

        public Matrix3 getLastTransform() {
            if (this._lastTransform == null) {
                return null;
            }

            Matrix3 result = Matrix3.makeTrans(-this._lastOffset.dx, -this._lastOffset.dy);
            result.preConcat(this._lastTransform);
            return result;
        }

        Matrix3 _collectTransformForLayerChain(List<ContainerLayer> layers) {
            Matrix3 result = Matrix3.I();
            for (int index = layers.Count - 1; index > 0; index -= 1) {
                layers[index].applyTransform(layers[index - 1], result);
            }

            return result;
        }

        void _establishTransform() {
            D.assert(this.link != null);
            this._lastTransform = null;
            if (this.link._leader == null) {
                return;
            }

            D.assert(this.link.leader.owner == this.owner,
                "Linked LeaderLayer anchor is not in the same layer tree as the FollowerLayer.");
            D.assert(this.link.leader._lastOffset != null,
                "LeaderLayer anchor must come before FollowerLayer in paint order, but the reverse was true.");

            HashSet<Layer> ancestors = new HashSet<Layer>();
            Layer ancestor = this.parent;
            while (ancestor != null) {
                ancestors.Add(ancestor);
                ancestor = ancestor.parent;
            }

            ContainerLayer layer = this.link.leader;
            List<ContainerLayer> forwardLayers = new List<ContainerLayer> {null, layer};
            do {
                layer = layer.parent;
                forwardLayers.Add(layer);
            } while (!ancestors.Contains(layer));

            ancestor = layer;

            layer = this;
            List<ContainerLayer> inverseLayers = new List<ContainerLayer> {layer};
            do {
                layer = layer.parent;
                inverseLayers.Add(layer);
            } while (layer != ancestor);

            Matrix3 forwardTransform = this._collectTransformForLayerChain(forwardLayers);
            Matrix3 inverseTransform = this._collectTransformForLayerChain(inverseLayers);
            var inverse = Matrix3.I();
            var invertible = inverseTransform.invert(inverse);
            if (!invertible) {
                return;
            }

            inverseTransform = inverse;
            inverseTransform.preConcat(forwardTransform);
            inverseTransform.preTranslate(this.linkedOffset.dx, this.linkedOffset.dy);
            this._lastTransform = inverseTransform;
        }

        protected override bool alwaysNeedsAddToScene {
            get { return true; }
        }

        internal override flow.Layer addToScene(SceneBuilder builder, Offset layerOffset = null) {
            layerOffset = layerOffset ?? Offset.zero;

            D.assert(this.link != null);
            if (this.link.leader == null && !this.showWhenUnlinked) {
                this._lastTransform = null;
                this._lastOffset = null;
                return null;
            }

            this._establishTransform();
            if (this._lastTransform != null) {
                builder.pushTransform(this._lastTransform);
                this.addChildrenToScene(builder);
                builder.pop();
                this._lastOffset = this.unlinkedOffset + layerOffset;
            } else {
                this._lastOffset = null;
                var matrix = Matrix3.makeTrans(this.unlinkedOffset.dx, this.unlinkedOffset.dy);
                builder.pushTransform(matrix);
                this.addChildrenToScene(builder);
                builder.pop();
            }

            return null;
        }

        public override void applyTransform(Layer child, Matrix3 transform) {
            D.assert(child != null);
            D.assert(transform != null);
            if (this._lastTransform != null) {
                transform.preConcat(this._lastTransform);
            } else {
                transform.preConcat(Matrix3.makeTrans(this.unlinkedOffset.dx, this.unlinkedOffset.dy));
            }
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new DiagnosticsProperty<LayerLink>("link", this.link));
            properties.add(new TransformProperty("transform", this.getLastTransform(),
                defaultValue: Diagnostics.kNullDefaultValue));
        }
    }
}