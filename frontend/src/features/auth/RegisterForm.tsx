import { useState } from "react";
import { useNavigate } from "react-router-dom";
import InputBox from "../../components/InputBox";
import { registerSchema } from "./schema";
import { MapZodErrorsToFields, type FieldErrors } from "./MapZodErrorsToFields";

type RegisterFormFieldsErrors = "email" | "password" | "confirmPassword";

export default function RegisterForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<FieldErrors<RegisterFormFieldsErrors>>(
    {}
  );
  const [confirmPassword, setConfirmPassword] = useState("");
  const navigate = useNavigate();

  const resetForm = () => {
    setEmail("");
    setPassword("");
    setConfirmPassword("");
    setErrors({});
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const formResult = registerSchema.safeParse({
      email,
      password,
      confirmPassword,
    });

    // if the form is not valid
    if (!formResult.success) {
      setErrors(MapZodErrorsToFields(formResult.error));
      return;
    }

    // if the form is valid
    setErrors({});

    const AuthResponse = {
      token: "token",
      role: "user",
    };

    localStorage.setItem("token", AuthResponse.token);
    localStorage.setItem("role", AuthResponse.role);

    resetForm();

    navigate("/home");
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 rounded-lg shadow-lg max-w-md w-full">
        <h2 className="text-2xl font-bold mb-7 text-center text-gray-800">
          Register
        </h2>

        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <InputBox
              id="email"
              label="Email"
              type="email"
              value={email}
              setValue={setEmail}
              placeholder="Enter your email"
              error={errors.email}
            />
          </div>

          <div className="mb-6">
            <InputBox
              id="password"
              label="Password"
              type="password"
              value={password}
              setValue={setPassword}
              placeholder="Enter your password"
              error={errors.password}
            />
          </div>

          <div className="mb-6">
            <InputBox
              id="confirmPassword"
              label="Confirm Password"
              type="password"
              value={confirmPassword}
              setValue={setConfirmPassword}
              placeholder="Confirm your password"
              error={errors.confirmPassword}
            />
          </div>

          <div className="flex items-center justify-between ">
            <button
              className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded cursor-pointer"
              type="submit"
            >
              Register
            </button>

            <button
              type="button"
              className="text-sm text-blue-500 hover:text-blue-700 cursor-pointer"
              onClick={() => navigate("/login")}
            >
              Back to Login as User
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
