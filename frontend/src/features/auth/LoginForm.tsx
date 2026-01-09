import { useState } from "react";
import { useNavigate } from "react-router-dom";
import InputBox from "../../components/InputBox";
import { loginSchema } from "./schema";
import {
  MapZodErrorsToFields,
  type FieldErrors,
} from "../../utils/MapZodErrorsToFields";
import { login as loginApi } from "../../services/authApi";
import type { ILoginResponse } from "../../types/IAuth";
import PopupBox from "../../components/PopupBox";
import type { IApiError } from "../../types/IApiResponse";
import { useAuth } from "../../hooks/useAuth";
import Spinner from "../../components/Spinner";
import { IRole } from "../../types/IRole";

type LoginFormFieldsErrors = "email" | "password";

export default function LoginForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<FieldErrors<LoginFormFieldsErrors>>({});
  const [errorPopup, setErrorPopup] = useState({ open: false, message: "" });
  const closeErrorPopup = () => setErrorPopup({ open: false, message: "" });
  const navigate = useNavigate();
  const { login, user, isLoading } = useAuth();

  if (isLoading) {
    return <Spinner />;
  }

  if (user?.scopes === IRole.PhotographyCompany) {
    navigate("/dashboard");
    return;
  } else if (user?.scopes === IRole.Agent) {
    navigate("/my-property");
    return;
  }

  const resetForm = () => {
    setEmail("");
    setPassword("");
    setErrors({});
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const formResult = loginSchema.safeParse({ email, password });

    // if the form is not valid
    if (!formResult.success) {
      setErrors(MapZodErrorsToFields(formResult.error));
      return;
    }

    // if the form is valid
    setErrors({});

    try {
      const res: ILoginResponse = await loginApi({ email, password });

      login(res.data);

      resetForm();

      if (user?.scopes === IRole.PhotographyCompany) {
        navigate("/dashboard");
      } else if (user?.scopes === IRole.Agent) {
        navigate("/my-property");
      }
    } catch (error: unknown) {
      setErrorPopup({ open: true, message: (error as IApiError).title });
    }
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 rounded-lg shadow-lg max-w-md w-full">
        <h2 className="text-2xl font-bold mb-7 text-center text-gray-800">
          Login
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
              Login
            </button>

            <button
              type="button"
              className="text-sm text-blue-500 hover:text-blue-700 cursor-pointer"
              onClick={() => navigate("/register")}
            >
              Don't have an account? Register
            </button>
          </div>
        </form>
      </div>
      <PopupBox
        message={errorPopup.message}
        open={errorPopup.open}
        onClose={closeErrorPopup}
      />
    </div>
  );
}
