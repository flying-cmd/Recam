import { useState } from "react";
import { useNavigate } from "react-router-dom";
import InputBox from "../../components/InputBox";
import { loginSchema } from "./schema";
import { MapZodErrorsToFields, type FieldErrors } from "./MapZodErrorsToFields";

interface LoginFormProps {
  title: "Login" | "Register" | "Admin";
  submitButtonText: string;
}

type LoginFormFieldsErrors = "email" | "password";

export default function LoginForm({ title, submitButtonText }: LoginFormProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<FieldErrors<LoginFormFieldsErrors>>({});
  const navigate = useNavigate();

  const resetForm = () => {
    setEmail("");
    setPassword("");
    setErrors({});
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const formResult = loginSchema.safeParse({ email, password });

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
          {title}
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

          <div className="flex items-center justify-between ">
            <button
              className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded cursor-pointer"
              type="submit"
            >
              {submitButtonText}
            </button>

            {title === "Login" ? (
              <>
                <button
                  type="button"
                  className="text-sm text-blue-500 hover:text-blue-700 cursor-pointer"
                  onClick={() => navigate("/register")}
                >
                  Don't have an account? Register
                </button>

                <button
                  type="button"
                  className="text-sm text-blue-500 hover:text-blue-700 cursor-pointer"
                  onClick={() => navigate("/admin/login")}
                >
                  Login as Admin
                </button>
              </>
            ) : (
              <button
                type="button"
                className="text-sm text-blue-500 hover:text-blue-700 cursor-pointer"
                onClick={() => navigate("/login")}
              >
                Login as User
              </button>
            )}
          </div>
        </form>
      </div>
    </div>
  );
}
