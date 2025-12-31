import { BrowserRouter, Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import AdminLogin from "./pages/auth/AdminLogin";
import AuthProvider from "./features/auth/AuthProvider";
import ProtectedRoute from "./components/ProtectedRoute";
import Unauthorized from "./pages/auth/Unauthorized";
import RoleBasedRoute from "./components/RoleBasedRoute";
import AdminPannel from "./pages/admin/AdminPannel";
import Spinner from "./components/Spinner";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/admin/login" element={<AdminLogin />} />
          <Route path="/unauthorized" element={<Unauthorized />} />
          <Route path="/loading" element={<Spinner />} />
          <Route
            path="/home"
            element={
              <ProtectedRoute>
                <Home />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin"
            element={
              <RoleBasedRoute scopes="admin">
                <AdminPannel />
              </RoleBasedRoute>
            }
          />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
