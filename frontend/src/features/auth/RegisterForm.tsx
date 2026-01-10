import { useState } from "react";
import { useNavigate } from "react-router-dom";
import InputBox from "../../components/InputBox";
import { registerSchema } from "./registerSchema";
import {
  MapZodErrorsToFields,
  type FieldErrors,
} from "../../utils/MapZodErrorsToFields";
import { register } from "../../services/authApi";
import type { IRegisterResponse } from "../../types/IAuth";
import type { IApiError } from "../../types/IApiResponse";
import PopupBox from "../../components/PopupBox";

type RegisterFormFieldsErrors =
  | "photographyCompanyName"
  | "email"
  | "phoneNumber"
  | "password"
  | "confirmPassword";

export default function RegisterForm() {
  const [photographyCompanyName, setPhotographyCompanyName] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [errors, setErrors] = useState<FieldErrors<RegisterFormFieldsErrors>>(
    {}
  );
  const [errorPopup, setErrorPopup] = useState({ open: false, message: "" });
  const closeErrorPopup = () => setErrorPopup({ open: false, message: "" });
  const navigate = useNavigate();

  const resetForm = () => {
    setPhotographyCompanyName("");
    setEmail("");
    setPhoneNumber("");
    setPassword("");
    setConfirmPassword("");
    setErrors({});
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const formResult = registerSchema.safeParse({
      photographyCompanyName,
      email,
      phoneNumber,
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

    try {
      const res: IRegisterResponse = await register({
        photographyCompanyName,
        email,
        phoneNumber,
        password,
        confirmPassword,
      });

      localStorage.setItem("token", res.data);

      resetForm();
      navigate("/dashboard");
    } catch (error: unknown) {
      console.error(error);
      setErrorPopup({ open: true, message: (error as IApiError).title });
    }
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
              id="photographyCompanyName"
              label="Photography Company Name"
              type="text"
              value={photographyCompanyName}
              setValue={setPhotographyCompanyName}
              placeholder="Enter your photography company name"
              error={errors.photographyCompanyName}
            />
          </div>

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

          <div className="mb-4">
            <InputBox
              id="phoneNumber"
              label="Phone Number"
              type="tel"
              value={phoneNumber}
              setValue={setPhoneNumber}
              placeholder="Enter your phone number"
              error={errors.phoneNumber}
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
              Back to Login
            </button>
          </div>
        </form>

        <PopupBox
          message={errorPopup.message}
          open={errorPopup.open}
          onClose={closeErrorPopup}
        />
      </div>
    </div>
  );
}
