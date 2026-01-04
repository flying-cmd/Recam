import { OctagonX, SquarePen } from "lucide-react";
import type { CaseContact } from "../../types/IListingCase";

interface CaseContactsProps {
  caseContacts: CaseContact[];
}

export default function CaseContacts({ caseContacts }: CaseContactsProps) {
  return (
    <div className="flex flex-col items-center mb-10">
      <h1 className="text-center font-bold text-2xl mt-20">Case Contacts</h1>

      <div className="w-140 flex flex-col items-center">
        {caseContacts.map((contact) => (
          <div
            key={contact.contactId}
            className="border-0 bg-gray-100 rounded-md my-3 flex flex-row justify-between w-full h-20"
          >
            <div className="flex flex-row justify-start items-center">
              {/* Avatar and Name */}
              {/* Avatar */}
              <div
                // shrink-0: Prevents the element from shrinking when it’s inside a flex container
                className="h-10 w-10 shrink-0 overflow-hidden rounded-full m-2"
              >
                {contact.profileImage ? (
                  <img
                    src={contact.profileImage}
                    alt="Profile image"
                    className="w-full h-full object-fill"
                    loading="lazy"
                  />
                ) : (
                  <img
                    src="/anonymous_avatar.jpg"
                    alt="Profile image"
                    className="w-full h-full object-fill"
                    loading="lazy"
                  />
                )}
              </div>

              {/* Name and Company Name */}
              <div className="mt-1">
                <p>
                  {contact.firstName} {contact.lastName}
                </p>
                <p className="text-gray-500">{contact.companyName}</p>
              </div>
            </div>

            {/* Email and Phone Number */}
            <div className="mt-1 mr-2 flex flex-row items-center justify-end">
              <div>
                <p className="text-right">{contact.phoneNumber}</p>
                <p className="text-gray-500 text-right">{contact.email}</p>
              </div>

              {/* Edit Button */}
              <div className="ml-2 overflow-hidden rounded-full shrink-0 flex justify-center items-center w-9 h-9 bg-blue-400">
                <SquarePen color="white" />
              </div>

              {/* Delete Button */}
              <div className="ml-2 flex justify-center items-center overflow-hidden rounded-full shrink-0 w-9 h-9 bg-red-400">
                <OctagonX color="white" />
              </div>
            </div>
          </div>
        ))}

        <div>
          <button>
            <span className="text-sm font-medium underline underline-offset-2 hover:text-blue-500">
              Click to add
            </span>
          </button>
        </div>
      </div>
    </div>
  );
}
