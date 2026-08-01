import { Composition } from "remotion";
import { DemoGGP, DEMO_DURATION } from "./DemoGGP";

export const MyComposition = () => {
  return (
    <Composition
      id="DemoGGP"
      component={DemoGGP}
      durationInFrames={DEMO_DURATION}
      fps={30}
      width={1920}
      height={1080}
    />
  );
};
