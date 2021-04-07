using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Engine.Utils.MathUtils
{
    public static class Matrix4x4Extensions
    {
		// Adapted from https://github.com/luca-piccioni/OpenGL.Net/blob/540aacd305c7bd2e6b7148cadc6574b49fe9a458/OpenGL.Net/Matrix.cs
		public static Matrix4x4 LookAtDirection(Vector3 eyePosition, Vector3 forwardVector, Vector3 upVector)
		{
			Vector3 rightVector;

			forwardVector = forwardVector.Normalized();

			rightVector = Vector3.Cross(forwardVector, upVector.Normalized());
			if (rightVector.Length() <= 0.0f)
				rightVector = Vector3.UnitX;
			else
				rightVector = rightVector.Normalized();

			upVector = (Vector3.Cross(rightVector, forwardVector)).Normalized();

			// Compute view matrix
			Matrix4x4 r = new Matrix4x4();

			// Row 0: right vector
			r.M11= rightVector.X;
			r.M21 = rightVector.Y;
			r.M31 = rightVector.Z;
			// Row 1: up vector
			r.M12 = upVector.X;
			r.M22 = upVector.Y;
			r.M32 = upVector.Z;
			// Row 2: opposite of forward vector
			r.M13 = -forwardVector.X;
			r.M23 = -forwardVector.Y;
			r.M33 = -forwardVector.Z;

			r.M44 = 1.0f;

			// Eye position
			r *= Matrix4x4.CreateTranslation(-eyePosition);

			return r;
		}
	}
}
