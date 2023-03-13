// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Animations.ExpressionsFork
{
    /// <summary>
    /// Class CompositionExtensions.
    /// </summary>
    public static class CompositionExtensions
    {
        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>AmbientLightReferenceNode.</returns>
        public static AmbientLightReferenceNode GetReference(this AmbientLight compObj)
        {
            return new AmbientLightReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>ColorBrushReferenceNode.</returns>
        public static ColorBrushReferenceNode GetReference(this CompositionColorBrush compObj)
        {
            return new ColorBrushReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>DistantLightReferenceNode.</returns>
        public static DistantLightReferenceNode GetReference(this DistantLight compObj)
        {
            return new DistantLightReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>DropShadowReferenceNode.</returns>
        public static DropShadowReferenceNode GetReference(this DropShadow compObj)
        {
            return new DropShadowReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>InsetClipReferenceNode.</returns>
        public static InsetClipReferenceNode GetReference(this InsetClip compObj)
        {
            return new InsetClipReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>InteractionTrackerReferenceNode.</returns>
        public static InteractionTrackerReferenceNode GetReference(this InteractionTracker compObj)
        {
            return new InteractionTrackerReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>NineGridBrushReferenceNode.</returns>
        public static NineGridBrushReferenceNode GetReference(this CompositionNineGridBrush compObj)
        {
            return new NineGridBrushReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>PointLightReferenceNode.</returns>
        public static PointLightReferenceNode GetReference(this PointLight compObj)
        {
            return new PointLightReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>PropertySetReferenceNode.</returns>
        public static PropertySetReferenceNode GetReference(this CompositionPropertySet compObj)
        {
            return new PropertySetReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>SpotLightReferenceNode.</returns>
        public static SpotLightReferenceNode GetReference(this SpotLight compObj)
        {
            return new SpotLightReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>SurfaceBrushReferenceNode.</returns>
        public static SurfaceBrushReferenceNode GetReference(this CompositionSurfaceBrush compObj)
        {
            return new SurfaceBrushReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this CompositionObject.
        /// </summary>
        /// <param name="compObj">The comp object.</param>
        /// <returns>VisualReferenceNode.</returns>
        public static VisualReferenceNode GetReference(this Visual compObj)
        {
            return new VisualReferenceNode(null, compObj);
        }

        /// <summary>
        /// Create an ExpressionNode reference to this specialized PropertySet.
        /// </summary>
        /// <typeparam name="T">A class that derives from PropertySetReferenceNode.</typeparam>
        /// <param name="ps">The ps.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.Exception">Invalid property set specialization</exception>
        public static T GetSpecializedReference<T>(this CompositionPropertySet ps)
            where T : PropertySetReferenceNode
        {
            if (typeof(T) == typeof(ManipulationPropertySetReferenceNode))
            {
                return new ManipulationPropertySetReferenceNode(null, ps) as T;
            }
            else if (typeof(T) == typeof(PointerPositionPropertySetReferenceNode))
            {
                return new PointerPositionPropertySetReferenceNode(null, ps) as T;
            }
            else
            {
                throw new System.Exception("Invalid property set specialization");
            }
        }

        /// <summary>
        /// Connects the specified ExpressionNode with the specified property of the object.
        /// </summary>
        /// <param name="compObject">The comp object.</param>
        /// <param name="propertyName">The name of the property that the expression will target.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void StartAnimation(this CompositionObject compObject, string propertyName, ExpressionNode expressionNode)
        {
            compObject.StartAnimation(propertyName, CreateExpressionAnimationFromNode(compObject.Compositor, expressionNode));
        }

        /// <summary>
        /// Inserts a KeyFrame whose value is calculated using the specified ExpressionNode.
        /// </summary>
        /// <param name="keyframeAnimation">The keyframe animation.</param>
        /// <param name="normalizedProgressKey">The time the key frame should occur at, expressed as a percentage of the animation Duration. Allowed value is from 0.0 to 1.0.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        /// <param name="easing">The easing function to use when interpolating between frames.</param>
        public static void InsertExpressionKeyFrame(this KeyFrameAnimation keyframeAnimation, float normalizedProgressKey, ExpressionNode expressionNode, CompositionEasingFunction easing = null)
        {
            expressionNode.ClearReferenceInfo();

            keyframeAnimation.InsertExpressionKeyFrame(normalizedProgressKey, expressionNode.ToExpressionString(), easing);

            expressionNode.SetAllParameters(keyframeAnimation);
        }

        /// <summary>
        /// Use the value of specified ExpressionNode to determine if this inertia modifier should be chosen.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetCondition(this InteractionTrackerInertiaRestingValue modifier, ExpressionNode expressionNode)
        {
            modifier.Condition = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary>
        /// Use the value of specified ExpressionNode as the resting value for this inertia modifier.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetRestingValue(this InteractionTrackerInertiaRestingValue modifier, ExpressionNode expressionNode)
        {
            modifier.RestingValue = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary>
        /// Use the value of specified ExpressionNode to determine if this inertia modifier should be chosen.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetCondition(this InteractionTrackerInertiaMotion modifier, ExpressionNode expressionNode)
        {
            modifier.Condition = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary>
        /// Use the value of specified ExpressionNode to dictate the motion for this inertia modifier.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetMotion(this InteractionTrackerInertiaMotion modifier, ExpressionNode expressionNode)
        {
            modifier.Motion = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary>
        /// Use the value of specified ExpressionNode to determine if this composition conditional value modifier should be chosen.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetCondition(this CompositionConditionalValue modifier, ExpressionNode expressionNode)
        {
            modifier.Condition = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary>
        /// Use the value of specified ExpressionNode as the value for this composition conditional value
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetValue(this CompositionConditionalValue modifier, ExpressionNode expressionNode)
        {
            modifier.Value = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary>
        /// Creates the expression animation from node.
        /// </summary>
        /// <param name="compositor">The compositor.</param>
        /// <param name="expressionNode">The expression node.</param>
        /// <returns>ExpressionAnimation.</returns>
        private static ExpressionAnimation CreateExpressionAnimationFromNode(Compositor compositor, ExpressionNode expressionNode)
        {
            // Only create a new animation if this node hasn't already generated one before, so we don't have to re-parse the expression string.
            if (expressionNode.ExpressionAnimation == null)
            {
                expressionNode.ClearReferenceInfo();
                expressionNode.ExpressionAnimation = compositor.CreateExpressionAnimation(expressionNode.ToExpressionString());
            }

            // We need to make sure all parameters are up to date, even if the animation already existed.
            expressionNode.SetAllParameters(expressionNode.ExpressionAnimation);

            return expressionNode.ExpressionAnimation;
        }

        internal static float EvaluateSubchannel(this ExpressionNode node, string subchannel) => (node, subchannel) switch
        {
            (Vector2Node n, "X") => n.Evaluate().X,
            (Vector2Node n, "Y") => n.Evaluate().Y,

            (Vector3Node n, "X") => n.Evaluate().X,
            (Vector3Node n, "Y") => n.Evaluate().Y,
            (Vector3Node n, "Z") => n.Evaluate().Z,

            (Vector4Node n, "X") => n.Evaluate().X,
            (Vector4Node n, "Y") => n.Evaluate().Y,
            (Vector4Node n, "Z") => n.Evaluate().Z,
            (Vector4Node n, "W") => n.Evaluate().W,

            (Matrix3x2Node n, "Channel11") => n.Evaluate().M11,
            (Matrix3x2Node n, "Channel12") => n.Evaluate().M12,
            (Matrix3x2Node n, "Channel21") => n.Evaluate().M21,
            (Matrix3x2Node n, "Channel22") => n.Evaluate().M22,
            (Matrix3x2Node n, "Channel31") => n.Evaluate().M31,
            (Matrix3x2Node n, "Channel32") => n.Evaluate().M32,

            (Matrix4x4Node n, "Channel11") => n.Evaluate().M11,
            (Matrix4x4Node n, "Channel12") => n.Evaluate().M12,
            (Matrix4x4Node n, "Channel13") => n.Evaluate().M13,
            (Matrix4x4Node n, "Channel14") => n.Evaluate().M14,
            (Matrix4x4Node n, "Channel21") => n.Evaluate().M21,
            (Matrix4x4Node n, "Channel22") => n.Evaluate().M22,
            (Matrix4x4Node n, "Channel23") => n.Evaluate().M23,
            (Matrix4x4Node n, "Channel24") => n.Evaluate().M24,
            (Matrix4x4Node n, "Channel31") => n.Evaluate().M31,
            (Matrix4x4Node n, "Channel32") => n.Evaluate().M32,
            (Matrix4x4Node n, "Channel33") => n.Evaluate().M33,
            (Matrix4x4Node n, "Channel34") => n.Evaluate().M34,
            (Matrix4x4Node n, "Channel41") => n.Evaluate().M41,
            (Matrix4x4Node n, "Channel42") => n.Evaluate().M42,
            (Matrix4x4Node n, "Channel43") => n.Evaluate().M43,
            (Matrix4x4Node n, "Channel44") => n.Evaluate().M44,

            _ => 0
        };
    }
}
