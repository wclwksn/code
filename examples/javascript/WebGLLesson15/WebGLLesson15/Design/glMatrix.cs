using ScriptCoreLib;
using System;
using ScriptCoreLib.JavaScript.WebGL;
using System.ComponentModel;

namespace WebGLLesson15.Design
{
    [Script]
    [Description("Future versions of JSC will enable seamless integration with JavaScript libraries")]
    internal class __glMatrix : glMatrix_0_9_5_min
    {
        // this should be generated via assets build :)

        [Script(ExternalTarget = "mat4")]
        static public mat4 mat4;

        [Script(ExternalTarget = "mat3")]
        static public mat3 mat3;

        [Script(ExternalTarget = "vec3")]
        static public vec3 vec3;
    }

    #region natives

    [Script(HasNoPrototype = true, ExternalTarget = "mat3")]
    internal class mat3
    {
        internal Float32Array create()
        {
            throw new NotImplementedException();
        }

        internal void transpose(object normalMatrix)
        {
            throw new NotImplementedException();
        }
    }

    [Script(HasNoPrototype = true, ExternalTarget = "vec3")]
    internal class vec3
    {
        internal Float32Array create()
        {
            throw new NotImplementedException();
        }

        internal void normalize(float[] lightingDirection, object adjustedLD)
        {
            throw new NotImplementedException();
        }

        internal void scale(object adjustedLD, int p)
        {
            throw new NotImplementedException();
        }
    }

    [Script(HasNoPrototype = true, ExternalTarget = "mat4")]
    internal class mat4
    {
        // ouch. we cannot use static like we want to :)

        // see also: http://code.google.com/p/glmatrix/wiki/Usage

        public Float32Array create()
        {
            throw new NotImplementedException();
        }

        public void translate(Float32Array mvMatrix, params float[] p)
        {
            throw new NotImplementedException();
        }

        public void identity(Float32Array mvMatrix)
        {
            throw new NotImplementedException();
        }

        public void perspective(float p, float p_2, float p_3, float p_4, Float32Array pMatrix)
        {
            throw new NotImplementedException();
        }



        internal void set(Float32Array mvMatrix, Float32Array copy)
        {
            throw new NotImplementedException();
        }

        internal void rotate(Float32Array mvMatrix, float p, params float[] p_2)
        {
            throw new NotImplementedException();
        }

        internal void toInverseMat3(Float32Array mvMatrix, object normalMatrix)
        {
            throw new NotImplementedException();
        }

        internal void multiply(Float32Array newRotationMatrix, Float32Array moonRotationMatrix, Float32Array moonRotationMatrix_2)
        {
            throw new NotImplementedException();
        }

        internal void multiply(Float32Array mvMatrix, Float32Array moonRotationMatrix)
        {
            throw new NotImplementedException();
        }
    }



    #endregion

}