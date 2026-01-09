import { BrowserRouter, Route, Routes } from "react-router-dom";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import AuthProvider from "./features/auth/AuthProvider";
import Unauthorized from "./pages/auth/Unauthorized";
import RoleBasedRoute from "./components/RoleBasedRoute";
import PropertyDetails from "./pages/photographyCompany/PropertyDetails";
import AppLayout from "./pages/AppLayout";
import AgentPage from "./pages/photographyCompany/AgentPage";
import ListingCasePage from "./pages/photographyCompany/ListingCasePage";
import EditListingCasePage from "./pages/photographyCompany/EditListingCasePage";
import AgentPannel from "./pages/agent/AgentPannel";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route element={<AppLayout />}>
            <Route path="login" element={<Login />} />
            <Route path="register" element={<Register />} />
            <Route path="unauthorized" element={<Unauthorized />} />

            {/* Protected routes (PhotographyCompany) */}
            <Route element={<RoleBasedRoute scopes="PhotographyCompany" />}>
              <Route path="dashboard" element={<ListingCasePage />} />
              <Route path="listings" element={<ListingCasePage />} />
              <Route path="agents" element={<AgentPage />} />
              <Route
                path="property-edit/:listingCaseId"
                element={<EditListingCasePage />}
              />
              <Route
                path="listings/:listingCaseId"
                element={<PropertyDetails />}
              />
            </Route>

            {/* Protected routes (Agent) */}
            <Route element={<RoleBasedRoute scopes="Agent" />}>
              <Route path="my-property" element={<AgentPannel />} />
            </Route>
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
